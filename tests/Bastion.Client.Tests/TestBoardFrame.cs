using System;
using Microsoft.Xna.Framework;
using Xunit;
using Bastion.Client.Rendering;
using Bastion.Domain;

namespace Bastion.Client.Tests;

// The steps mimic the board asset: one unit per cell, columns along X and rows
// running towards negative Z, with the first groove crossing at (-3.5, 0, 3.5).
public sealed class TestBoardFrame
{
    private const float AlmostHalfACell = 0.49f;
    private const float PastHalfACell = 0.51f;
    private const float SomeHeight = 3.0f;

    private static BoardFrame CreateFrame()
    {
        return new BoardFrame(new Vector3(-3.5f, 0.0f, 3.5f), Vector3.UnitX, -Vector3.UnitZ);
    }

    [Fact]
    public void GetGroovePosition_FirstCrossing_IsTheOrigin()
    {
        var expected = new Vector3(-3.5f, 0.0f, 3.5f);

        Vector3 position = CreateFrame().GetGroovePosition(new BoardPosition(0, 0));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetGroovePosition_LastCrossing_IsTheOppositeCorner()
    {
        var expected = new Vector3(3.5f, 0.0f, -3.5f);

        Vector3 position = CreateFrame().GetGroovePosition(new BoardPosition(7, 7));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetCellPosition_FirstCell_IsHalfACellBeforeItsCrossing()
    {
        var expected = new Vector3(-4.0f, 0.0f, 4.0f);

        Vector3 position = CreateFrame().GetCellPosition(new BoardPosition(0, 0));

        Assert.Equal(expected, position);
    }

    // The last column and the last row have no crossing of their own, so the
    // position has to come out of extrapolating the steps.
    [Fact]
    public void GetCellPosition_LastCell_IsFoundWithoutACrossing()
    {
        var expected = new Vector3(4.0f, 0.0f, -4.0f);

        Vector3 position = CreateFrame().GetCellPosition(new BoardPosition(8, 8));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetCellPosition_MiddleCell_IsTheCentreOfTheBoard()
    {
        Vector3 position = CreateFrame().GetCellPosition(new BoardPosition(4, 4));

        Assert.Equal(Vector3.Zero, position);
    }

    [Fact]
    public void Up_ColumnsAndRowsOnThePlane_PointsAwayFromTheBoard()
    {
        Vector3 up = CreateFrame().Up;

        Assert.Equal(Vector3.Up, up);
    }

    [Fact]
    public void CellSize_OneUnitPerCell_IsOne()
    {
        float size = CreateFrame().CellSize;

        Assert.Equal(1.0f, size);
    }

    // The exporter may write the board in another unit. Nothing is hard coded, so
    // a board in centimetres only changes what a cell measures.
    [Fact]
    public void CellSize_ABoardExportedInCentimetres_IsAHundred()
    {
        var frame = new BoardFrame(new Vector3(-350.0f, 0.0f, 350.0f), Vector3.UnitX * 100.0f, -Vector3.UnitZ * 100.0f);

        float size = frame.CellSize;

        Assert.Equal(100.0f, size);
    }

    // Nor does it assume which way the rows run: with them reversed, the normal
    // of the board points the other way and the camera still sits above it.
    [Fact]
    public void Up_RowsRunningTheOtherWay_PointsTheOtherWay()
    {
        var frame = new BoardFrame(Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);

        Vector3 up = frame.Up;

        Assert.Equal(Vector3.Down, up);
    }

    // A board turned 45 degrees in its plane: the cell still lands on the grid it
    // defines, which is what reading the axes from the asset buys.
    [Fact]
    public void GetCellPosition_ABoardTurnedInItsPlane_FollowsTheTurnedGrid()
    {
        float diagonal = MathF.Sqrt(2.0f) / 2.0f;
        var column = new Vector3(diagonal, 0.0f, diagonal);
        var row = new Vector3(-diagonal, 0.0f, diagonal);
        var frame = new BoardFrame(Vector3.Zero, column, row);

        Vector3 position = frame.GetCellPosition(new BoardPosition(1, 1));

        Assert.Equal(0.0, Vector3.Distance(column * 0.5f + (row * 0.5f), position), 4);
    }

    // What the pointer lands on: the crossing nearest to the point it meets the
    // board at, so a wall rides on the one under the cursor.
    [Fact]
    public void GetCrossingAt_ThePointOfACrossing_IsThatCrossing()
    {
        BoardFrame frame = CreateFrame();
        var crossing = new BoardPosition(5, 2);

        BoardPosition found = frame.GetCrossingAt(frame.GetGroovePosition(crossing));

        Assert.Equal(crossing, found);
    }

    [Fact]
    public void GetCrossingAt_JustPastACrossing_IsStillThatCrossing()
    {
        BoardFrame frame = CreateFrame();
        var crossing = new BoardPosition(5, 2);
        Vector3 nudged = frame.GetGroovePosition(crossing) + (Vector3.UnitX * AlmostHalfACell);

        BoardPosition found = frame.GetCrossingAt(nudged);

        Assert.Equal(crossing, found);
    }

    [Fact]
    public void GetCrossingAt_PastTheMiddleTowardsTheNext_IsTheNextCrossing()
    {
        BoardFrame frame = CreateFrame();
        var crossing = new BoardPosition(5, 2);
        Vector3 nudged = frame.GetGroovePosition(crossing) + (Vector3.UnitX * PastHalfACell);

        BoardPosition found = frame.GetCrossingAt(nudged);

        Assert.Equal(new BoardPosition(crossing.Column + 1, crossing.Row), found);
    }

    // The height of the point does not matter: what counts is where it falls on
    // the plane of the board.
    [Fact]
    public void GetCrossingAt_APointAboveTheBoard_IsTheCrossingUnderIt()
    {
        BoardFrame frame = CreateFrame();
        var crossing = new BoardPosition(3, 6);
        Vector3 above = frame.GetGroovePosition(crossing) + (Vector3.Up * SomeHeight);

        BoardPosition found = frame.GetCrossingAt(above);

        Assert.Equal(crossing, found);
    }

    [Fact]
    public void ColumnAxis_AnyScale_IsAUnitVector()
    {
        var frame = new BoardFrame(Vector3.Zero, Vector3.UnitX * 100.0f, -Vector3.UnitZ * 100.0f);

        float length = frame.ColumnAxis.Length();

        Assert.Equal(1.0f, length);
    }
}
