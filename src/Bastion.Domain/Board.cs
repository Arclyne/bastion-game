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

    // A wall covers two cells from the crossing it starts on (CU-21 RN-03).
    private const int WallLength = 2;

    private readonly HashSet<Wall> _walls;
    private readonly HashSet<BoardPosition> _pawns;

    public Board(int size, IEnumerable<Wall> walls, IEnumerable<BoardPosition> pawns)
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

    public IReadOnlyCollection<BoardPosition> Pawns => _pawns;

    public bool IsInside(BoardPosition cell)
    {
        return cell.Column >= 0 && cell.Column < Size && cell.Row >= 0 && cell.Row < Size;
    }

    public bool HasPawn(BoardPosition cell)
    {
        return _pawns.Contains(cell);
    }

    // The two cells are taken to be neighbours, which is the only way a pawn
    // travels.
    public bool IsWallBetween(BoardPosition from, BoardPosition to)
    {
        if (from.Column == to.Column)
        {
            int groove = Math.Min(from.Row, to.Row);

            return _walls.Any(wall => IsHorizontalAcross(wall, groove, from.Column));
        }

        int crossing = Math.Min(from.Column, to.Column);

        return _walls.Any(wall => IsVerticalAcross(wall, crossing, from.Row));
    }

    private static bool IsHorizontalAcross(Wall wall, int groove, int column)
    {
        return wall.Orientation == WallOrientation.Horizontal
            && wall.Crossing.Row == groove
            && IsLineCovered(wall.Crossing.Column, column);
    }

    private static bool IsVerticalAcross(Wall wall, int crossing, int row)
    {
        return wall.Orientation == WallOrientation.Vertical
            && wall.Crossing.Column == crossing
            && IsLineCovered(wall.Crossing.Row, row);
    }

    private static bool IsLineCovered(int start, int line)
    {
        return line >= start && line < start + WallLength;
    }
}
