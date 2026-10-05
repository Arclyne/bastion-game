using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

public sealed class TestWall
{
    private static readonly BoardPosition _crossing = new BoardPosition(3, 4);

    [Fact]
    public void Turn_AHorizontalWall_RunsTheOtherWay()
    {
        var wall = new Wall(_crossing, WallOrientation.Horizontal);

        Wall turned = wall.Turn();

        Assert.Equal(WallOrientation.Vertical, turned.Orientation);
    }

    [Fact]
    public void Turn_AVerticalWall_RunsTheOtherWay()
    {
        var wall = new Wall(_crossing, WallOrientation.Vertical);

        Wall turned = wall.Turn();

        Assert.Equal(WallOrientation.Horizontal, turned.Orientation);
    }

    [Fact]
    public void Turn_AnyWall_StaysOnTheSameCrossing()
    {
        var wall = new Wall(_crossing, WallOrientation.Horizontal);

        Wall turned = wall.Turn();

        Assert.Equal(_crossing, turned.Crossing);
    }

    [Fact]
    public void Turn_Twice_IsTheWallItStartedAs()
    {
        var wall = new Wall(_crossing, WallOrientation.Horizontal);

        Wall turned = wall.Turn().Turn();

        Assert.Equal(wall, turned);
    }
}
