using System;
using System.Collections.Generic;
using System.Linq;

namespace Bastion.Domain;

// The state a move is decided against. It will also hold what a match changes,
// as walls are placed and pawns advance (CU-20, CU-21).
public sealed class Board
{
    // Two cells per side is the smallest board where a move and a wall still
    // mean something.
    public const int MinimumSize = 2;

    private readonly HashSet<Wall> _walls;
    private readonly List<Pawn> _pawns;

    public Board(int size, IEnumerable<Wall> walls, IEnumerable<Pawn> pawns)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(size, MinimumSize);
        ArgumentNullException.ThrowIfNull(walls);
        ArgumentNullException.ThrowIfNull(pawns);

        Size = size;
        _walls = [.. walls];
        _pawns = [.. pawns];
    }

    public int Size { get; }

    public IReadOnlyCollection<Wall> Walls => _walls;

    public IReadOnlyList<Pawn> Pawns => _pawns;

    // One crossing fewer per side than cells, since a wall needs two cells to
    // lie on.
    public int CrossingCount => Size - Wall.Length + 1;

    public bool IsInside(BoardPosition cell)
    {
        return cell.Column >= 0 && cell.Column < Size && cell.Row >= 0 && cell.Row < Size;
    }

    public bool IsCrossing(BoardPosition crossing)
    {
        return crossing.Column >= 0 && crossing.Column < CrossingCount
            && crossing.Row >= 0 && crossing.Row < CrossingCount;
    }

    public bool HasPawn(BoardPosition cell)
    {
        return _pawns.Exists(pawn => pawn.Cell == cell);
    }

    // Zero steps away means the cell already is on that side, which is how a
    // player wins and where a path ends (CU-21 RN-05).
    public bool IsOnSide(BoardPosition cell, BoardSide side)
    {
        return GetStepsToSide(cell, side) == 0;
    }

    // How many rows or columns are left to that side. A pawn covers at most one
    // of them per move, so this never overstates the distance and can be used to
    // steer a search towards the goal.
    public int GetStepsToSide(BoardPosition cell, BoardSide side)
    {
        return side switch
        {
            BoardSide.Bottom => cell.Row,
            BoardSide.Top => Size - 1 - cell.Row,
            BoardSide.Left => cell.Column,
            BoardSide.Right => Size - 1 - cell.Column,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unknown board side.")
        };
    }

    public bool HasWall(Wall wall)
    {
        return _walls.Contains(wall);
    }

    // A placed wall is never taken back (CU-21 RN-07).
    public void Place(Wall wall)
    {
        _walls.Add(wall);
    }

    // The pawn keeps the goal it was given: moving changes where it stands, never
    // which side it is trying to reach.
    public void MovePawn(BoardPosition from, BoardPosition to)
    {
        int index = _pawns.FindIndex(pawn => pawn.Cell == from);
        if (index < 0)
        {
            return;
        }

        _pawns[index] = _pawns[index] with { Cell = to };
    }

    // The two cells are taken to be neighbours, which is the only way a pawn
    // travels.
    public bool IsWallBetween(BoardPosition from, BoardPosition to)
    {
        return IsWallBetween(from, to, null);
    }

    // The candidate is a wall that is only being considered, so it is not on the
    // board and has to be read on top of it (CU-21 step 13).
    public bool IsWallBetween(BoardPosition from, BoardPosition to, Wall? candidate)
    {
        return _walls.Any(wall => wall.IsBetween(from, to))
            || (candidate is not null && candidate.Value.IsBetween(from, to));
    }
}
