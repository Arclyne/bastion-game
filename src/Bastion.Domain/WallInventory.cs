using System;

namespace Bastion.Domain;

// CU-21 RN-02: every player is given a number of walls fixed by the mode, or by
// the host of a private room (CU-18 RN-04). It is given rather than split from a
// total, because quick does not come to twenty the way the other modes do.
public sealed class WallInventory
{
    // The range a private room may be set to (CU-18 RN-04), which is also what
    // the Room table admits.
    public const int LeastWallsPerPlayer = 1;
    public const int MostWallsPerPlayer = 20;

    private const int LeastPlayerCount = 1;

    private readonly int[] _remainingWalls;

    public WallInventory(int playerCount, int wallsPerPlayer)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(playerCount, LeastPlayerCount);
        ArgumentOutOfRangeException.ThrowIfLessThan(wallsPerPlayer, LeastWallsPerPlayer);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(wallsPerPlayer, MostWallsPerPlayer);

        _remainingWalls = new int[playerCount];
        Array.Fill(_remainingWalls, wallsPerPlayer);
    }

    public int PlayerCount => _remainingWalls.Length;

    public int GetRemainingWalls(int playerIndex)
    {
        CheckThePlayer(playerIndex);

        return _remainingWalls[playerIndex];
    }

    public bool HasWallsLeft(int playerIndex)
    {
        return GetRemainingWalls(playerIndex) > 0;
    }

    // Refusing keeps a modified client from putting one wall more on the board
    // than the mode allows (CU-21 FA-09).
    public void Spend(int playerIndex)
    {
        if (!HasWallsLeft(playerIndex))
        {
            throw new InvalidOperationException($"No walls left to spend. PlayerIndex={playerIndex}");
        }

        _remainingWalls[playerIndex]--;
    }

    private void CheckThePlayer(int playerIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(playerIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(playerIndex, PlayerCount);
    }
}
