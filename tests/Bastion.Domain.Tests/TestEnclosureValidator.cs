using System;
using System.Collections.Generic;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// The validator keeps the way it finds for each player, so several of these
// tests ask twice on purpose: the second answer is the one that comes from the
// way already known.
public sealed class TestEnclosureValidator
{
    private const int ClassicSize = 9;
    private const int Centre = ClassicSize / 2;
    private const int LastLine = ClassicSize - 1;

    private const int FirstPlayer = 0;

    // Straight up a column with nothing in the way, which is one cell per row.
    private const int StraightWayLength = ClassicSize;

    // Leaving the column and climbing the next one costs exactly one cell more.
    private const int DetouredWayLength = StraightWayLength + 1;
    private const int Matches = 300;
    private const int TurnsPerMatch = 24;
    private const int PlayerCount = 3;

    // Fixed so a match that fails can be played again.
    private const int ComparisonSeed = 777;

    private static readonly Pawn _cornerPawn = new Pawn(new BoardPosition(0, 0), BoardSide.Top);
    private static readonly Pawn _startingPawn = new Pawn(new BoardPosition(Centre, 0), BoardSide.Top);
    private static readonly Pawn _rivalPawn = new Pawn(new BoardPosition(Centre, LastLine), BoardSide.Bottom);
    private static readonly Pawn _cornerRival = new Pawn(new BoardPosition(0, LastLine), BoardSide.Bottom);

    // Far from every way through, so it changes nothing for anybody.
    private static readonly Wall _farAway = new Wall(new BoardPosition(6, 6), WallOrientation.Vertical);

    // Lying above the bottom left corner. On its own it only forces a detour; it
    // is the wall beside the corner that shuts the corner in.
    private static readonly Wall _overTheCorner =
        new Wall(new BoardPosition(0, 1), WallOrientation.Horizontal);

    private static readonly Wall _besideTheCorner =
        new Wall(new BoardPosition(1, 0), WallOrientation.Vertical);

    // The same pair of walls at the other end of the board, which shuts in a
    // rival standing in the top left corner.
    private static readonly Wall _underTheFarCorner =
        new Wall(new BoardPosition(0, LastLine - 2), WallOrientation.Horizontal);

    private static readonly Wall _besideTheFarCorner =
        new Wall(new BoardPosition(1, LastLine - 1), WallOrientation.Vertical);

    // Lies across the straight way up the middle column and leaves the ways
    // around it open.
    private static readonly Wall _overTheMiddle =
        new Wall(new BoardPosition(Centre, 0), WallOrientation.Horizontal);

    private static EnclosureValidator CreateValidator(IEnumerable<Wall> walls, IEnumerable<Pawn> pawns)
    {
        return new EnclosureValidator(new Board(ClassicSize, walls, pawns));
    }

    [Fact]
    public void FindShutInPlayer_AWallNowhereNearAWayThrough_NamesNobody()
    {
        EnclosureValidator validator = CreateValidator([], [_startingPawn, _rivalPawn]);

        BoardSide? shutIn = validator.FindShutInPlayer(_farAway);

        Assert.Null(shutIn);
    }

    [Fact]
    public void FindShutInPlayer_TheSameWallAskedAgain_NamesNobodyAgain()
    {
        EnclosureValidator validator = CreateValidator([], [_startingPawn, _rivalPawn]);
        validator.FindShutInPlayer(_farAway);

        BoardSide? shutIn = validator.FindShutInPlayer(_farAway);

        Assert.Null(shutIn);
    }

    // The wall cuts the way both players were using, which is what sends the
    // search out for another one.
    [Fact]
    public void FindShutInPlayer_AWallThatCutsTheKnownWayButLeavesAnother_NamesNobody()
    {
        EnclosureValidator validator = CreateValidator([], [_startingPawn, _rivalPawn]);
        validator.FindShutInPlayer(_farAway);

        BoardSide? shutIn = validator.FindShutInPlayer(_overTheMiddle);

        Assert.Null(shutIn);
    }

    [Fact]
    public void FindShutInPlayer_AWallThatShutsAPawnIn_NamesThatPlayer()
    {
        EnclosureValidator validator = CreateValidator([_overTheCorner], [_cornerPawn, _rivalPawn]);

        BoardSide? shutIn = validator.FindShutInPlayer(_besideTheCorner);

        Assert.Equal(BoardSide.Top, shutIn);
    }

    // The way already known runs through the very step this wall takes away, so
    // trusting it would let the wall through.
    [Fact]
    public void FindShutInPlayer_AWallThatShutsAPawnInAfterAWayWasFound_NamesThatPlayer()
    {
        EnclosureValidator validator = CreateValidator([_overTheCorner], [_cornerPawn, _rivalPawn]);
        validator.FindShutInPlayer(_farAway);

        BoardSide? shutIn = validator.FindShutInPlayer(_besideTheCorner);

        Assert.Equal(BoardSide.Top, shutIn);
    }

    // CU-21 RN-05: the check covers every player, not only the one next to the
    // wall.
    [Fact]
    public void FindShutInPlayer_AWallThatShutsTheRivalIn_NamesTheRival()
    {
        EnclosureValidator validator = CreateValidator([_underTheFarCorner], [_startingPawn, _cornerRival]);

        BoardSide? shutIn = validator.FindShutInPlayer(_besideTheFarCorner);

        Assert.Equal(BoardSide.Bottom, shutIn);
    }

    // The way is kept for the player, not for the cell it started from, so a pawn
    // that has moved on is asked about where it stands now.
    [Fact]
    public void FindShutInPlayer_APawnThatMovedIntoTheCornerBeingSealed_NamesThatPlayer()
    {
        var board = new Board(ClassicSize, [_overTheCorner], [new Pawn(new BoardPosition(2, 0), BoardSide.Top)]);
        var validator = new EnclosureValidator(board);
        validator.FindShutInPlayer(_farAway);
        board.MovePawn(new BoardPosition(2, 0), new BoardPosition(1, 0));

        BoardSide? shutIn = validator.FindShutInPlayer(_besideTheCorner);

        Assert.Equal(BoardSide.Top, shutIn);
    }

    // Whole matches played out, with walls going down and pawns moving between
    // questions, in which the validator that keeps paths is asked exactly what a
    // brand new one that has kept nothing is asked. Keeping a path is only ever an
    // optimisation, so the two must never disagree; three players are used so at
    // least two of them share a goal side.
    [Fact]
    public void FindShutInPlayer_MatchesPlayedOut_AgreesWithAValidatorThatKeepsNothing()
    {
        List<string> mismatches = FindMismatches(AddWhenTheAnswersDiffer);

        Assert.Empty(mismatches);
    }

    // The two matches played out run on the same seed, so the boards they reach
    // are the same and only what is checked on them differs.
    private static List<string> FindMismatches(Action<List<string>, RandomPieceFactory> playAMatch)
    {
        var pieces = new RandomPieceFactory(ComparisonSeed, ClassicSize);
        var mismatches = new List<string>();

        for (int match = 0; match < Matches; match++)
        {
            playAMatch(mismatches, pieces);
        }

        return mismatches;
    }

    private static void AddWhenTheAnswersDiffer(List<string> mismatches, RandomPieceFactory pieces)
    {
        var board = new Board(ClassicSize, [], CreatePawns(pieces));
        var keeper = new EnclosureValidator(board);

        for (int turn = 0; turn < TurnsPerMatch; turn++)
        {
            Wall candidate = pieces.CreateWall();
            BoardSide? fromTheKeeper = keeper.FindShutInPlayer(candidate);
            BoardSide? fromScratch = new EnclosureValidator(board).FindShutInPlayer(candidate);

            AddWhenDifferent(mismatches, fromTheKeeper, fromScratch);
            PlayTheTurn(board, candidate, fromScratch is null);
            MoveAPawn(board, pieces);
        }
    }

    private static void AddWhenDifferent(List<string> mismatches, BoardSide? fromTheKeeper, BoardSide? fromScratch)
    {
        if (fromTheKeeper != fromScratch)
        {
            mismatches.Add($"keeper={fromTheKeeper} fresh={fromScratch}");
        }
    }

    private static void PlayTheTurn(Board board, Wall candidate, bool isAllowed)
    {
        if (isAllowed)
        {
            board.Place(candidate);
        }
    }

    // Moves are not asked to be legal: all this needs is for a pawn to stand
    // somewhere else, so the path kept for it stops starting where it is.
    private static void MoveAPawn(Board board, RandomPieceFactory pieces)
    {
        Pawn pawn = board.Pawns[pieces.Pick(board.Pawns.Count)];
        BoardPosition next = pawn.Cell.Step(BoardStep.StraightSteps[pieces.Pick(BoardStep.StraightSteps.Count)]);

        if (board.IsInside(next) && !board.IsWallBetween(pawn.Cell, next))
        {
            board.MovePawn(pawn.Cell, next);
        }
    }

    private static Pawn[] CreatePawns(RandomPieceFactory pieces)
    {
        var pawns = new List<Pawn>();

        for (int playerIndex = 0; playerIndex < PlayerCount; playerIndex++)
        {
            pawns.Add(pieces.CreatePawn());
        }

        return [.. pawns];
    }

    // CU-21 RN-09 reads the shortest way, so a detour found around a wall that was
    // merely being considered must never be mistaken for it.
    [Fact]
    public void FindShortestPath_AWallOnlyBeingConsidered_IsStillTheWayThatIgnoresThatWall()
    {
        var board = new Board(ClassicSize, [], [_startingPawn]);
        var validator = new EnclosureValidator(board);
        validator.FindShutInPlayer(_overTheMiddle);

        IReadOnlyList<BoardPosition>? shortestCells = validator.FindShortestPath(FirstPlayer);

        Assert.Equal(StraightWayLength, shortestCells?.Count);
    }

    [Fact]
    public void FindShortestPath_AfterTheWallWentDown_IsAsLongAsTheDetourItForces()
    {
        var board = new Board(ClassicSize, [], [_startingPawn]);
        var validator = new EnclosureValidator(board);
        validator.FindShutInPlayer(_overTheMiddle);
        board.Place(_overTheMiddle);

        IReadOnlyList<BoardPosition>? shortestCells = validator.FindShortestPath(FirstPlayer);

        Assert.Equal(DetouredWayLength, shortestCells?.Count);
    }

    // The wall was carried to another crossing and let go of there, so the detour
    // found around where it used to be was worth nothing.
    [Fact]
    public void FindShortestPath_TheWallLetGoSomewhereElse_IgnoresTheDetourFoundBefore()
    {
        var board = new Board(ClassicSize, [], [_startingPawn]);
        var validator = new EnclosureValidator(board);
        validator.FindShutInPlayer(_overTheMiddle);
        validator.FindShutInPlayer(_farAway);
        board.Place(_farAway);

        IReadOnlyList<BoardPosition>? shortestCells = validator.FindShortestPath(FirstPlayer);

        Assert.Equal(StraightWayLength, shortestCells?.Count);
    }

    [Fact]
    public void FindShortestPath_MatchesPlayedOut_IsAlwaysTheWayAFreshSearchFinds()
    {
        List<string> mismatches = FindMismatches(AddWhenTheShortestWaysDiffer);

        Assert.Empty(mismatches);
    }

    private static void AddWhenTheShortestWaysDiffer(List<string> mismatches, RandomPieceFactory pieces)
    {
        var board = new Board(ClassicSize, [], CreatePawns(pieces));
        var keeper = new EnclosureValidator(board);

        for (int turn = 0; turn < TurnsPerMatch; turn++)
        {
            Wall candidate = pieces.CreateWall();
            PlayTheTurn(board, candidate, keeper.FindShutInPlayer(candidate) is null);
            MoveAPawn(board, pieces);
            AddWhenAPlayerDiffers(mismatches, board, keeper);
        }
    }

    private static void AddWhenAPlayerDiffers(List<string> mismatches, Board board, EnclosureValidator keeper)
    {
        for (int playerIndex = 0; playerIndex < board.Pawns.Count; playerIndex++)
        {
            int kept = keeper.FindShortestPath(playerIndex)?.Count ?? FloodFill.NoWay;
            int fresh = new PathFinder(board).FindPath(board.Pawns[playerIndex])?.Count ?? FloodFill.NoWay;

            if (kept != fresh)
            {
                mismatches.Add($"player={playerIndex} kept={kept} fresh={fresh}");
            }
        }
    }

    [Fact]
    public void FindShutInPlayer_ABoardWithNoPawns_NamesNobody()
    {
        EnclosureValidator validator = CreateValidator([], []);

        BoardSide? shutIn = validator.FindShutInPlayer(_besideTheCorner);

        Assert.Null(shutIn);
    }
}
