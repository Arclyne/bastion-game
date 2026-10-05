using System;
using Xunit;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// CU-21 RN-02, with the two numbers the modes fix for a nine by nine board: ten
// walls each for two players and five each for four.
public sealed class TestWallInventory
{
    private const int ClassicPlayerCount = 2;
    private const int ClassicWallsPerPlayer = 10;
    private const int FourPlayerCount = 4;
    private const int FourPlayerWallsPerPlayer = 5;
    private const int FirstPlayer = 0;
    private const int SecondPlayer = 1;

    private static WallInventory CreateClassicInventory()
    {
        return new WallInventory(ClassicPlayerCount, ClassicWallsPerPlayer);
    }

    private static WallInventory CreateSpentInventory()
    {
        WallInventory inventory = CreateClassicInventory();

        for (int wallIndex = 0; wallIndex < ClassicWallsPerPlayer; wallIndex++)
        {
            inventory.Spend(FirstPlayer);
        }

        return inventory;
    }

    [Fact]
    public void GetRemainingWalls_AClassicMatch_GivesTenToEachPlayer()
    {
        int remaining = CreateClassicInventory().GetRemainingWalls(FirstPlayer);

        Assert.Equal(ClassicWallsPerPlayer, remaining);
    }

    [Fact]
    public void GetRemainingWalls_AFourPlayerMatch_GivesFiveToEachPlayer()
    {
        var inventory = new WallInventory(FourPlayerCount, FourPlayerWallsPerPlayer);

        int remaining = inventory.GetRemainingWalls(FourPlayerCount - 1);

        Assert.Equal(FourPlayerWallsPerPlayer, remaining);
    }

    [Fact]
    public void PlayerCount_AFourPlayerMatch_HasASupplyForEverySeat()
    {
        var inventory = new WallInventory(FourPlayerCount, FourPlayerWallsPerPlayer);

        Assert.Equal(FourPlayerCount, inventory.PlayerCount);
    }

    [Fact]
    public void HasWallsLeft_ANewInventory_IsTrue()
    {
        bool hasWallsLeft = CreateClassicInventory().HasWallsLeft(FirstPlayer);

        Assert.True(hasWallsLeft);
    }

    [Fact]
    public void GetRemainingWalls_OneWallSpent_IsOneFewer()
    {
        WallInventory inventory = CreateClassicInventory();

        inventory.Spend(FirstPlayer);

        int remaining = inventory.GetRemainingWalls(FirstPlayer);

        Assert.Equal(ClassicWallsPerPlayer - 1, remaining);
    }

    // One player spending says nothing about what the others still hold.
    [Fact]
    public void GetRemainingWalls_AnotherPlayerSpending_LeavesThisOneAlone()
    {
        WallInventory inventory = CreateClassicInventory();

        inventory.Spend(FirstPlayer);

        int remaining = inventory.GetRemainingWalls(SecondPlayer);

        Assert.Equal(ClassicWallsPerPlayer, remaining);
    }

    [Fact]
    public void GetRemainingWalls_EveryWallSpent_IsNone()
    {
        int remaining = CreateSpentInventory().GetRemainingWalls(FirstPlayer);

        Assert.Equal(0, remaining);
    }

    [Fact]
    public void HasWallsLeft_EveryWallSpent_IsFalse()
    {
        bool hasWallsLeft = CreateSpentInventory().HasWallsLeft(FirstPlayer);

        Assert.False(hasWallsLeft);
    }

    // The interface never offers it, so getting here means the gate was skipped
    // and a modified client is at work (CU-21 FA-09).
    [Fact]
    public void Spend_WithNothingLeftToSpend_IsRefused()
    {
        WallInventory inventory = CreateSpentInventory();

        void Spend() => inventory.Spend(FirstPlayer);

        Assert.Throws<InvalidOperationException>(Spend);
    }

    [Fact]
    public void GetRemainingWalls_APlayerThatIsNotPlaying_IsRejected()
    {
        WallInventory inventory = CreateClassicInventory();

        void GetRemainingWalls() => inventory.GetRemainingWalls(ClassicPlayerCount);

        Assert.Throws<ArgumentOutOfRangeException>(GetRemainingWalls);
    }

    [Fact]
    public void WallInventory_MoreWallsThanAPrivateRoomMayAskFor_IsRejected()
    {
        void Create() => new WallInventory(ClassicPlayerCount, WallInventory.MostWallsPerPlayer + 1);

        Assert.Throws<ArgumentOutOfRangeException>(Create);
    }

    [Fact]
    public void WallInventory_FewerWallsThanAPrivateRoomMayAskFor_IsRejected()
    {
        void Create() => new WallInventory(ClassicPlayerCount, WallInventory.LeastWallsPerPlayer - 1);

        Assert.Throws<ArgumentOutOfRangeException>(Create);
    }

    [Fact]
    public void WallInventory_NoPlayersAtAll_IsRejected()
    {
        void Create() => new WallInventory(0, ClassicWallsPerPlayer);

        Assert.Throws<ArgumentOutOfRangeException>(Create);
    }
}
