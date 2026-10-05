using System.Collections.Generic;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// CU-21 RN-04: walls neither overlap nor cross.
public sealed class TestWallPlacementValidator
{
    private const int ClassicSize = 9;
    private const int CrossingsPerSide = ClassicSize - 1;

    private static readonly BoardPosition _crossing = new BoardPosition(3, 4);

    private static WallPlacementResult Check(Wall placed, Wall wall)
    {
        var board = new Board(ClassicSize, [placed], []);

        return new WallPlacementValidator(board).Check(wall);
    }

    private static WallPlacementResult CheckOnAnEmptyBoard(Wall wall)
    {
        var board = new Board(ClassicSize, [], []);

        return new WallPlacementValidator(board).Check(wall);
    }

    [Fact]
    public void Check_AnEmptyBoard_AllowsTheWall()
    {
        var wall = new Wall(_crossing, WallOrientation.Horizontal);

        WallPlacementResult result = CheckOnAnEmptyBoard(wall);

        Assert.Equal(WallPlacementResult.Allowed, result);
    }

    [Fact]
    public void Check_TheSameCrossingAndTheSameWay_Overlaps()
    {
        var placed = new Wall(_crossing, WallOrientation.Horizontal);

        WallPlacementResult result = Check(placed, placed);

        Assert.Equal(WallPlacementResult.Overlaps, result);
    }

    // A wall is two cells long, so the next crossing along shares one of them.
    [Fact]
    public void Check_AHorizontalWallOneColumnAlong_Overlaps()
    {
        var placed = new Wall(_crossing, WallOrientation.Horizontal);
        var crossing = new BoardPosition(_crossing.Column + 1, _crossing.Row);

        WallPlacementResult result = Check(placed, new Wall(crossing, WallOrientation.Horizontal));

        Assert.Equal(WallPlacementResult.Overlaps, result);
    }

    [Fact]
    public void Check_AHorizontalWallTwoColumnsAlong_IsAllowed()
    {
        var placed = new Wall(_crossing, WallOrientation.Horizontal);
        var crossing = new BoardPosition(_crossing.Column + Wall.Length, _crossing.Row);

        WallPlacementResult result = Check(placed, new Wall(crossing, WallOrientation.Horizontal));

        Assert.Equal(WallPlacementResult.Allowed, result);
    }

    [Fact]
    public void Check_AVerticalWallOneRowAlong_Overlaps()
    {
        var placed = new Wall(_crossing, WallOrientation.Vertical);
        var crossing = new BoardPosition(_crossing.Column, _crossing.Row + 1);

        WallPlacementResult result = Check(placed, new Wall(crossing, WallOrientation.Vertical));

        Assert.Equal(WallPlacementResult.Overlaps, result);
    }

    [Fact]
    public void Check_AVerticalWallTwoRowsAlong_IsAllowed()
    {
        var placed = new Wall(_crossing, WallOrientation.Vertical);
        var crossing = new BoardPosition(_crossing.Column, _crossing.Row + Wall.Length);

        WallPlacementResult result = Check(placed, new Wall(crossing, WallOrientation.Vertical));

        Assert.Equal(WallPlacementResult.Allowed, result);
    }

    // The same crossing with the two running different ways is the crossing of
    // walls that the rule names apart from the overlap.
    [Fact]
    public void Check_TheSameCrossingTheOtherWay_Crosses()
    {
        var placed = new Wall(_crossing, WallOrientation.Horizontal);

        WallPlacementResult result = Check(placed, new Wall(_crossing, WallOrientation.Vertical));

        Assert.Equal(WallPlacementResult.Crosses, result);
    }

    // Side by side and running different ways they share no cell, so they fit.
    [Fact]
    public void Check_TheOtherWayOnTheNextCrossing_IsAllowed()
    {
        var placed = new Wall(_crossing, WallOrientation.Horizontal);
        var crossing = new BoardPosition(_crossing.Column + 1, _crossing.Row);

        WallPlacementResult result = Check(placed, new Wall(crossing, WallOrientation.Vertical));

        Assert.Equal(WallPlacementResult.Allowed, result);
    }

    // There is one crossing fewer per side than cells: a wall on the last one
    // would hang off the board.
    [Fact]
    public void Check_ACrossingPastTheLastOne_IsOutsideTheBoard()
    {
        var crossing = new BoardPosition(CrossingsPerSide, 0);

        WallPlacementResult result = CheckOnAnEmptyBoard(new Wall(crossing, WallOrientation.Horizontal));

        Assert.Equal(WallPlacementResult.OutsideTheBoard, result);
    }

    [Fact]
    public void Check_TheLastCrossing_IsAllowed()
    {
        var crossing = new BoardPosition(CrossingsPerSide - 1, CrossingsPerSide - 1);

        WallPlacementResult result = CheckOnAnEmptyBoard(new Wall(crossing, WallOrientation.Horizontal));

        Assert.Equal(WallPlacementResult.Allowed, result);
    }

    [Fact]
    public void GetPlaceableCrossings_AnEmptyBoard_OffersEveryCrossing()
    {
        var validator = new WallPlacementValidator(new Board(ClassicSize, [], []));

        IReadOnlyList<BoardPosition> crossings = validator.GetPlaceableCrossings(WallOrientation.Horizontal);

        Assert.Equal(CrossingsPerSide * CrossingsPerSide, crossings.Count);
    }

    [Fact]
    public void GetPlaceableCrossings_AWallOnTheBoard_DropsTheCrossingItTakesUp()
    {
        var board = new Board(ClassicSize, [new Wall(_crossing, WallOrientation.Horizontal)], []);
        var validator = new WallPlacementValidator(board);

        IReadOnlyList<BoardPosition> crossings = validator.GetPlaceableCrossings(WallOrientation.Horizontal);

        Assert.DoesNotContain(_crossing, crossings);
    }

    // Rotating asks again, and the crossing next to a horizontal wall is only
    // out of reach for another horizontal one.
    [Fact]
    public void GetPlaceableCrossings_AfterTurningTheWall_AnswersForTheNewOrientation()
    {
        var board = new Board(ClassicSize, [new Wall(_crossing, WallOrientation.Horizontal)], []);
        var validator = new WallPlacementValidator(board);
        var held = new Wall(new BoardPosition(_crossing.Column + 1, _crossing.Row), WallOrientation.Horizontal);

        IReadOnlyList<BoardPosition> crossings = validator.GetPlaceableCrossings(held.Turn().Orientation);

        Assert.Contains(held.Crossing, crossings);
    }
}
