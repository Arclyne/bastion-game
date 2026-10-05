using System.Collections.Generic;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// Every board here is a classic nine by nine one, and the pawn under test is the
// only one on it: other pawns do not stop a way through.
public sealed class TestPathFinder
{
    private const int ClassicSize = 9;
    private const int Centre = ClassicSize / 2;
    private const int LastLine = ClassicSize - 1;

    // Straight up a column with nothing in the way, which is one cell per row.
    private const int StraightWayLength = ClassicSize;

    // Leaving the column and climbing the next one costs exactly one cell more.
    private const int DetouredWayLength = StraightWayLength + 1;

    // Enough walls for the board to be cut up in interesting ways without every
    // board turning into a sealed one.
    private const int MostWalls = 14;
    private const int ComparisonRounds = 4000;

    // Fixed so a board that fails can be reached again.
    private const int ComparisonSeed = 20261005;

    private static readonly BoardPosition _corner = new BoardPosition(0, 0);
    private static readonly BoardPosition _middleOfTheBackRow = new BoardPosition(Centre, 0);

    // Two walls seal the bottom left corner: one lying above it and one standing
    // across the way out along the row.
    private static readonly Wall _overTheCorner =
        new Wall(new BoardPosition(0, 1), WallOrientation.Horizontal);

    private static readonly Wall _besideTheCorner =
        new Wall(new BoardPosition(1, 0), WallOrientation.Vertical);

    // Lies across the straight way up the middle column, leaving the ways around
    // it open.
    private static readonly Wall _overTheMiddle =
        new Wall(new BoardPosition(Centre, 0), WallOrientation.Horizontal);

    private static IReadOnlyList<BoardPosition>? FindPath(IEnumerable<Wall> walls, Pawn pawn)
    {
        return new PathFinder(new Board(ClassicSize, walls, [pawn])).FindPath(pawn);
    }

    private static IReadOnlyList<BoardPosition>? FindPathTo(BoardSide goal)
    {
        return FindPath([], new Pawn(new BoardPosition(Centre, Centre), goal));
    }

    [Fact]
    public void FindPath_AnEmptyBoard_EndsOnTheGoalSide()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPath([], new Pawn(_middleOfTheBackRow, BoardSide.Top));

        Assert.Equal(LastLine, pathCells?[^1].Row);
    }

    [Fact]
    public void FindPath_AnEmptyBoard_StartsWhereThePawnStands()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPath([], new Pawn(_middleOfTheBackRow, BoardSide.Top));

        Assert.Equal(_middleOfTheBackRow, pathCells?[0]);
    }

    // The heuristic never overstates how far the goal side is, so what comes back
    // is the shortest way and not merely one that works.
    [Fact]
    public void FindPath_AnEmptyBoard_TakesTheShortestWay()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPath([], new Pawn(_middleOfTheBackRow, BoardSide.Top));

        Assert.Equal(StraightWayLength, pathCells?.Count);
    }

    [Fact]
    public void FindPath_APawnAlreadyOnItsGoalSide_StaysWhereItStands()
    {
        IReadOnlyList<BoardPosition>? pathCells =
            FindPath([], new Pawn(_middleOfTheBackRow, BoardSide.Bottom));

        Assert.Equal(1, pathCells?.Count);
    }

    // Going around the wall costs the one step sideways it takes to leave the
    // column and nothing else.
    [Fact]
    public void FindPath_AWallAcrossTheStraightWay_GoesAroundIt()
    {
        IReadOnlyList<BoardPosition>? pathCells =
            FindPath([_overTheMiddle], new Pawn(_middleOfTheBackRow, BoardSide.Top));

        Assert.Equal(DetouredWayLength, pathCells?.Count);
    }

    [Fact]
    public void FindPath_APawnSealedInACorner_FindsNoWay()
    {
        IReadOnlyList<BoardPosition>? pathCells =
            FindPath([_overTheCorner, _besideTheCorner], new Pawn(_corner, BoardSide.Top));

        Assert.Null(pathCells);
    }

    // The same question asked of a wall that is only being considered, which is
    // what decides whether it may be let go of (CU-21 RN-05).
    [Fact]
    public void FindPath_AWallThatWouldSealTheLastWayOut_FindsNoWay()
    {
        var board = new Board(ClassicSize, [_overTheCorner], [new Pawn(_corner, BoardSide.Top)]);

        IReadOnlyList<BoardPosition>? pathCells =
            new PathFinder(board).FindPath(new Pawn(_corner, BoardSide.Top), _besideTheCorner);

        Assert.Null(pathCells);
    }

    [Fact]
    public void FindPath_AWallThatLeavesAWayOut_FindsIt()
    {
        var board = new Board(ClassicSize, [], [new Pawn(_middleOfTheBackRow, BoardSide.Top)]);

        IReadOnlyList<BoardPosition>? pathCells =
            new PathFinder(board).FindPath(new Pawn(_middleOfTheBackRow, BoardSide.Top), _overTheMiddle);

        Assert.Equal(DetouredWayLength, pathCells?.Count);
    }

    // All four sides, because the four player mode sends two players along the
    // columns instead of the rows.
    [Fact]
    public void FindPath_APawnHeadingForTheTop_EndsOnTheLastRow()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPathTo(BoardSide.Top);

        Assert.Equal(LastLine, pathCells?[^1].Row);
    }

    [Fact]
    public void FindPath_APawnHeadingForTheBottom_EndsOnTheFirstRow()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPathTo(BoardSide.Bottom);

        Assert.Equal(0, pathCells?[^1].Row);
    }

    [Fact]
    public void FindPath_APawnHeadingForTheRight_EndsOnTheLastColumn()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPathTo(BoardSide.Right);

        Assert.Equal(LastLine, pathCells?[^1].Column);
    }

    [Fact]
    public void FindPath_APawnHeadingForTheLeft_EndsOnTheFirstColumn()
    {
        IReadOnlyList<BoardPosition>? pathCells = FindPathTo(BoardSide.Left);

        Assert.Equal(0, pathCells?[^1].Column);
    }

    // Thousands of boards with walls thrown about, each answer measured against a
    // flood fill that searches in a wholly different way. It covers both halves of
    // the answer: whether the goal side can be reached at all, and the promise that
    // what comes back is the shortest way there.
    [Fact]
    public void FindPath_BoardsWithWallsThrownAbout_AgreesWithAFloodFill()
    {
        var pieces = new RandomPieceFactory(ComparisonSeed, ClassicSize);
        var mismatches = new List<string>();

        for (int round = 0; round < ComparisonRounds; round++)
        {
            AddWhenTheAnswersDiffer(mismatches, pieces);
        }

        Assert.Empty(mismatches);
    }

    private static void AddWhenTheAnswersDiffer(List<string> mismatches, RandomPieceFactory pieces)
    {
        Wall[] walls = CreateWalls(pieces);
        Pawn pawn = pieces.CreatePawn();
        var board = new Board(ClassicSize, walls, [pawn]);
        Wall? candidate = pieces.CreateCandidate();

        int measured = Measure(board, pawn, candidate);
        int expected = new FloodFill(board, candidate).MeasureWayTo(pawn);

        if (measured != expected)
        {
            mismatches.Add(
                $"walls={walls.Length} pawn={pawn} candidate={candidate} found={measured} expected={expected}");
        }
    }

    private static int Measure(Board board, Pawn pawn, Wall? candidate)
    {
        var finder = new PathFinder(board);
        if (candidate is null)
        {
            return finder.FindPath(pawn)?.Count ?? FloodFill.NoWay;
        }

        return finder.FindPath(pawn, candidate.Value)?.Count ?? FloodFill.NoWay;
    }

    // Walls are thrown on without asking whether they overlap: more walls in the
    // way is all this needs, and the geometry of a legal placement is CU-21 RN-04,
    // which is checked elsewhere.
    private static Wall[] CreateWalls(RandomPieceFactory pieces)
    {
        int count = pieces.Pick(MostWalls);
        var walls = new List<Wall>();

        for (int wallIndex = 0; wallIndex < count; wallIndex++)
        {
            walls.Add(pieces.CreateWall());
        }

        return [.. walls];
    }

    [Fact]
    public void FindPath_APawnOffTheBoard_FindsNoWay()
    {
        IReadOnlyList<BoardPosition>? pathCells =
            FindPath([], new Pawn(new BoardPosition(ClassicSize, 0), BoardSide.Top));

        Assert.Null(pathCells);
    }
}
