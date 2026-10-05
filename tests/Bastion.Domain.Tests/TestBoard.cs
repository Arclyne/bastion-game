using System;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// A wall lies on the crossing named by the cell at its lower left corner and
// covers that cell and the next one along its own axis.
public sealed class TestBoard
{
    private const int ClassicSize = 9;

    private static Board CreateBoard(Wall wall)
    {
        return new Board(ClassicSize, [wall], []);
    }

    [Fact]
    public void IsWallBetween_AHorizontalWallOnTheCellItStartsOn_StopsTheStepAcrossIt()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Horizontal));

        bool isBlocked = board.IsWallBetween(new BoardPosition(3, 4), new BoardPosition(3, 5));

        Assert.True(isBlocked);
    }

    [Fact]
    public void IsWallBetween_AHorizontalWallOnItsSecondCell_StopsTheStepAcrossIt()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Horizontal));

        bool isBlocked = board.IsWallBetween(new BoardPosition(4, 4), new BoardPosition(4, 5));

        Assert.True(isBlocked);
    }

    // Two cells long and no more: the third column is already past its end.
    [Fact]
    public void IsWallBetween_AHorizontalWallThreeColumnsAlong_LeavesTheStepOpen()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Horizontal));

        bool isBlocked = board.IsWallBetween(new BoardPosition(5, 4), new BoardPosition(5, 5));

        Assert.False(isBlocked);
    }

    // It lies along the columns, so it never stands in the way of a step that
    // travels along them.
    [Fact]
    public void IsWallBetween_AHorizontalWallAndAStepSideways_LeavesTheStepOpen()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Horizontal));

        bool isBlocked = board.IsWallBetween(new BoardPosition(3, 4), new BoardPosition(4, 4));

        Assert.False(isBlocked);
    }

    [Fact]
    public void IsWallBetween_AVerticalWallOnTheCellItStartsOn_StopsTheStepAcrossIt()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Vertical));

        bool isBlocked = board.IsWallBetween(new BoardPosition(3, 4), new BoardPosition(4, 4));

        Assert.True(isBlocked);
    }

    [Fact]
    public void IsWallBetween_AVerticalWallOnItsSecondCell_StopsTheStepAcrossIt()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Vertical));

        bool isBlocked = board.IsWallBetween(new BoardPosition(3, 5), new BoardPosition(4, 5));

        Assert.True(isBlocked);
    }

    [Fact]
    public void IsWallBetween_AVerticalWallAndAStepUpwards_LeavesTheStepOpen()
    {
        Board board = CreateBoard(new Wall(new BoardPosition(3, 4), WallOrientation.Vertical));

        bool isBlocked = board.IsWallBetween(new BoardPosition(3, 4), new BoardPosition(3, 5));

        Assert.False(isBlocked);
    }

    // The side a pawn is heading for, which is where a way through has to end
    // (CU-21 RN-05).
    [Fact]
    public void IsOnSide_ACellOnTheLastRow_IsOnTheTop()
    {
        var board = new Board(ClassicSize, [], []);

        bool isOnSide = board.IsOnSide(new BoardPosition(0, ClassicSize - 1), BoardSide.Top);

        Assert.True(isOnSide);
    }

    [Fact]
    public void IsOnSide_ACellOnTheLastRow_IsNotOnTheBottom()
    {
        var board = new Board(ClassicSize, [], []);

        bool isOnSide = board.IsOnSide(new BoardPosition(0, ClassicSize - 1), BoardSide.Bottom);

        Assert.False(isOnSide);
    }

    [Fact]
    public void GetStepsToSide_ACellOnTheFirstColumn_CountsEveryColumnToTheRight()
    {
        var board = new Board(ClassicSize, [], []);

        int steps = board.GetStepsToSide(new BoardPosition(0, 0), BoardSide.Right);

        Assert.Equal(ClassicSize - 1, steps);
    }

    [Fact]
    public void GetStepsToSide_ASideThatDoesNotExist_IsRejected()
    {
        var board = new Board(ClassicSize, [], []);

        void GetSteps() => board.GetStepsToSide(new BoardPosition(0, 0), (BoardSide)(-1));

        Assert.Throws<ArgumentOutOfRangeException>(GetSteps);
    }

    // Moving changes where a pawn stands, never the side it is trying to reach.
    [Fact]
    public void MovePawn_APawnOnTheBoard_KeepsTheGoalItWasGiven()
    {
        var pawn = new Pawn(new BoardPosition(4, 0), BoardSide.Top);
        var board = new Board(ClassicSize, [], [pawn]);

        board.MovePawn(pawn.Cell, new BoardPosition(4, 1));

        Assert.Equal(new Pawn(new BoardPosition(4, 1), BoardSide.Top), board.Pawns[0]);
    }

    [Fact]
    public void MovePawn_ACellWithNoPawnOnIt_LeavesThePawnsWhereTheyAre()
    {
        var pawn = new Pawn(new BoardPosition(4, 0), BoardSide.Top);
        var board = new Board(ClassicSize, [], [pawn]);

        board.MovePawn(new BoardPosition(0, 0), new BoardPosition(0, 1));

        Assert.Equal(pawn, board.Pawns[0]);
    }

    [Fact]
    public void IsInside_ACellPastTheLastColumn_IsFalse()
    {
        var board = new Board(ClassicSize, [], []);

        bool isInside = board.IsInside(new BoardPosition(ClassicSize, 0));

        Assert.False(isInside);
    }

    [Fact]
    public void IsInside_TheLastCell_IsTrue()
    {
        var board = new Board(ClassicSize, [], []);

        bool isInside = board.IsInside(new BoardPosition(ClassicSize - 1, ClassicSize - 1));

        Assert.True(isInside);
    }
}
