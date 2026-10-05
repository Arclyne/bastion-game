using System.Collections.Generic;
using System.Linq;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// The pawn under test always starts at the centre of the board.
public sealed class TestPawnMoveFinder
{
    private const int ClassicSize = 9;
    private const int Centre = ClassicSize / 2;
    private const int CornerMoveCount = 2;

    private static readonly BoardPosition _pawn = new BoardPosition(Centre, Centre);
    private static readonly BoardPosition _above = new BoardPosition(Centre, Centre + 1);
    private static readonly BoardPosition _below = new BoardPosition(Centre, Centre - 1);
    private static readonly BoardPosition _right = new BoardPosition(Centre + 1, Centre);
    private static readonly BoardPosition _left = new BoardPosition(Centre - 1, Centre);

    // Which side a pawn is heading for says nothing about where it may step, so
    // every pawn here is given the same goal.
    private static Board CreateBoard(IEnumerable<Wall> walls, IEnumerable<BoardPosition> cells)
    {
        return new Board(ClassicSize, walls, cells.Select(cell => new Pawn(cell, BoardSide.Top)));
    }

    private static BoardPosition[] GetMoves(IEnumerable<Wall> walls, IEnumerable<BoardPosition> rivals)
    {
        var pawns = new List<BoardPosition>(rivals) { _pawn };
        var finder = new PawnMoveFinder(CreateBoard(walls, pawns));

        return [.. finder.GetMoves(_pawn).OrderBy(cell => cell.Column).ThenBy(cell => cell.Row)];
    }

    [Fact]
    public void GetMoves_AnEmptyBoard_OffersTheFourNeighbours()
    {
        BoardPosition[] moves = GetMoves([], []);

        Assert.Equal([_left, _below, _above, _right], moves);
    }

    [Fact]
    public void GetMoves_FromACorner_OffersOnlyTheTwoNeighboursOnTheBoard()
    {
        var finder = new PawnMoveFinder(CreateBoard([], [new BoardPosition(0, 0)]));

        IReadOnlyList<BoardPosition> moves = finder.GetMoves(new BoardPosition(0, 0));

        Assert.Equal(CornerMoveCount, moves.Count);
    }

    // RN-02: a wall in between takes that neighbour away.
    [Fact]
    public void GetMoves_AWallAbove_DropsTheCellBehindIt()
    {
        BoardPosition[] moves = GetMoves([new Wall(_pawn, WallOrientation.Horizontal)], []);

        Assert.DoesNotContain(_above, moves);
    }

    // RN-04: the cell a rival stands on is never one of the moves.
    [Fact]
    public void GetMoves_ARivalAbove_NeverOffersTheCellItStandsOn()
    {
        BoardPosition[] moves = GetMoves([], [_above]);

        Assert.DoesNotContain(_above, moves);
    }

    // RN-03: nothing behind the rival, so the jump goes straight on.
    [Fact]
    public void GetMoves_ARivalWithAFreeCellBehindIt_OffersTheStraightJump()
    {
        BoardPosition[] moves = GetMoves([], [_above]);

        Assert.Contains(new BoardPosition(Centre, Centre + 2), moves);
    }

    [Fact]
    public void GetMoves_ARivalWithAFreeCellBehindIt_DoesNotOfferTheSidesOfTheRival()
    {
        BoardPosition[] moves = GetMoves([], [_above]);

        Assert.DoesNotContain(new BoardPosition(Centre + 1, Centre + 1), moves);
    }

    // RN-03: a wall behind the rival turns the jump into the two diagonals.
    [Fact]
    public void GetMoves_AWallBehindTheRival_OffersBothSidesOfTheRival()
    {
        var wall = new Wall(_above, WallOrientation.Horizontal);
        BoardPosition[] sides = [new BoardPosition(Centre - 1, Centre + 1), new BoardPosition(Centre + 1, Centre + 1)];

        BoardPosition[] moves = GetMoves([wall], [_above]);

        Assert.Equal(sides, moves.Where(sides.Contains).OrderBy(cell => cell.Column).ToArray());
    }

    [Fact]
    public void GetMoves_AWallBehindTheRival_DropsTheStraightJump()
    {
        BoardPosition[] moves = GetMoves([new Wall(_above, WallOrientation.Horizontal)], [_above]);

        Assert.DoesNotContain(new BoardPosition(Centre, Centre + 2), moves);
    }

    // RN-03 again: another pawn behind the rival counts the same as a wall.
    [Fact]
    public void GetMoves_AnotherPawnBehindTheRival_OffersTheSidesInsteadOfTheJump()
    {
        BoardPosition[] moves = GetMoves([], [_above, new BoardPosition(Centre, Centre + 2)]);

        Assert.Contains(new BoardPosition(Centre + 1, Centre + 1), moves);
    }

    [Fact]
    public void GetMoves_AWallBetweenThePawnAndTheRival_OffersNothingInThatDirection()
    {
        BoardPosition[] moves = GetMoves([new Wall(_pawn, WallOrientation.Horizontal)], [_above]);

        Assert.DoesNotContain(new BoardPosition(Centre, Centre + 2), moves);
    }

    // Not written down in CU-20: the edge of the board behind the rival is read
    // the same way as a wall, since there is nowhere to land.
    [Fact]
    public void GetMoves_ARivalOnTheLastRow_OffersTheSidesInsteadOfTheJump()
    {
        var pawn = new BoardPosition(Centre, ClassicSize - 2);
        var rival = new BoardPosition(Centre, ClassicSize - 1);
        var finder = new PawnMoveFinder(CreateBoard([], [pawn, rival]));

        IReadOnlyList<BoardPosition> moves = finder.GetMoves(pawn);

        Assert.Contains(new BoardPosition(Centre + 1, ClassicSize - 1), moves);
    }

    // Not written down either: a wall across one of the two sides takes that
    // side away and leaves the other one.
    [Fact]
    public void GetMoves_ASideOfTheRivalWalledOff_OffersOnlyTheOtherSide()
    {
        var behind = new Wall(_above, WallOrientation.Horizontal);
        var side = new Wall(_above, WallOrientation.Vertical);

        BoardPosition[] moves = GetMoves([behind, side], [_above]);

        Assert.DoesNotContain(new BoardPosition(Centre + 1, Centre + 1), moves);
    }

    [Fact]
    public void GetMoves_ASideOfTheRivalWalledOff_StillOffersTheOtherSide()
    {
        var behind = new Wall(_above, WallOrientation.Horizontal);
        var side = new Wall(_above, WallOrientation.Vertical);

        BoardPosition[] moves = GetMoves([behind, side], [_above]);

        Assert.Contains(new BoardPosition(Centre - 1, Centre + 1), moves);
    }
}
