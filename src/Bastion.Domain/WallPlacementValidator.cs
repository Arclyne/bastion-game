using System;
using System.Collections.Generic;
using System.Linq;

namespace Bastion.Domain;

// CU-21 RN-04: walls neither overlap nor cross each other. RN-05, which keeps a
// wall from shutting a player in, is not part of this check.
public sealed class WallPlacementValidator
{
    private readonly Board _board;

    public WallPlacementValidator(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        _board = board;
    }

    public WallPlacementResult Check(Wall wall)
    {
        if (!_board.IsCrossing(wall.Crossing))
        {
            return WallPlacementResult.OutsideTheBoard;
        }

        if (_board.Walls.Any(placed => IsCrossedBy(placed, wall)))
        {
            return WallPlacementResult.Crosses;
        }

        if (_board.Walls.Any(placed => IsOverlappedBy(placed, wall)))
        {
            return WallPlacementResult.Overlaps;
        }

        return WallPlacementResult.Allowed;
    }

    // Rotating the wall asks again, because the answer changes with the
    // orientation (CU-21 FA-03).
    public IReadOnlyList<BoardPosition> GetPlaceableCrossings(WallOrientation orientation)
    {
        var crossings = new List<BoardPosition>();

        foreach (BoardPosition crossing in GetEveryCrossing())
        {
            AddWhenPlaceable(crossings, new Wall(crossing, orientation));
        }

        return crossings;
    }

    private IEnumerable<BoardPosition> GetEveryCrossing()
    {
        for (int column = 0; column < _board.CrossingCount; column++)
        {
            for (int row = 0; row < _board.CrossingCount; row++)
            {
                yield return new BoardPosition(column, row);
            }
        }
    }

    private void AddWhenPlaceable(List<BoardPosition> crossings, Wall wall)
    {
        if (Check(wall) == WallPlacementResult.Allowed)
        {
            crossings.Add(wall.Crossing);
        }
    }

    private static bool IsCrossedBy(Wall placed, Wall wall)
    {
        return placed.Crossing == wall.Crossing && placed.Orientation != wall.Orientation;
    }

    // Running the same way, they overlap when they share a cell, which happens
    // on the same crossing and on the next one along the line they cover.
    private static bool IsOverlappedBy(Wall placed, Wall wall)
    {
        return placed.Orientation == wall.Orientation && IsWithinReach(placed.Crossing, wall);
    }

    private static bool IsWithinReach(BoardPosition crossing, Wall wall)
    {
        if (wall.Orientation == WallOrientation.Horizontal)
        {
            return crossing.Row == wall.Crossing.Row
                && Math.Abs(crossing.Column - wall.Crossing.Column) < Wall.Length;
        }

        return crossing.Column == wall.Crossing.Column
            && Math.Abs(crossing.Row - wall.Crossing.Row) < Wall.Length;
    }
}
