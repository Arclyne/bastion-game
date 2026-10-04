using Microsoft.Xna.Framework;
using Xunit;
using Bastion.Client.Rendering;

namespace Bastion.Client.Tests;

// The steps mimic the board asset: one unit per cell, columns along X and rows
// running towards negative Z, with the first groove crossing at (-3.5, 0, 3.5).
public sealed class TestBoardFrame
{
    private static BoardFrame CreateFrame()
    {
        return new BoardFrame(new Vector3(-3.5f, 0.0f, 3.5f), Vector3.UnitX, -Vector3.UnitZ);
    }

    [Fact]
    public void GetGroovePosition_FirstCrossing_IsTheOrigin()
    {
        var expected = new Vector3(-3.5f, 0.0f, 3.5f);

        Vector3 position = CreateFrame().GetGroovePosition(new BoardSlot(0, 0));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetGroovePosition_LastCrossing_IsTheOppositeCorner()
    {
        var expected = new Vector3(3.5f, 0.0f, -3.5f);

        Vector3 position = CreateFrame().GetGroovePosition(new BoardSlot(7, 7));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetCellPosition_FirstCell_IsHalfACellBeforeItsCrossing()
    {
        var expected = new Vector3(-4.0f, 0.0f, 4.0f);

        Vector3 position = CreateFrame().GetCellPosition(new BoardSlot(0, 0));

        Assert.Equal(expected, position);
    }

    // The last column and the last row have no crossing of their own, so the
    // position has to come out of extrapolating the steps.
    [Fact]
    public void GetCellPosition_LastCell_IsFoundWithoutACrossing()
    {
        var expected = new Vector3(4.0f, 0.0f, -4.0f);

        Vector3 position = CreateFrame().GetCellPosition(new BoardSlot(8, 8));

        Assert.Equal(expected, position);
    }

    [Fact]
    public void GetCellPosition_MiddleCell_IsTheCentreOfTheBoard()
    {
        Vector3 position = CreateFrame().GetCellPosition(new BoardSlot(4, 4));

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
}
