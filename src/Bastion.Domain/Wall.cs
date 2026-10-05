using System;

namespace Bastion.Domain;

// The crossing is named by the cell at its lower left corner, as the server
// stores it.
public readonly record struct Wall(BoardPosition Crossing, WallOrientation Orientation)
{
    // A wall measures exactly two cells (CU-21 RN-03).
    public const int Length = 2;

    // Rotating is what the player does before letting go of the wall, and it
    // changes which crossings it fits on (CU-21 FA-03).
    public Wall Turn()
    {
        WallOrientation turned = Orientation == WallOrientation.Horizontal
            ? WallOrientation.Vertical
            : WallOrientation.Horizontal;

        return this with { Orientation = turned };
    }

    // Whether the wall lies across the step between two neighbouring cells, which
    // is the only way it stops anything. It lives here so the board, the path
    // finder and a wall that is only being considered all ask the same question.
    public bool IsBetween(BoardPosition from, BoardPosition to)
    {
        if (from.Column == to.Column)
        {
            return Orientation == WallOrientation.Horizontal
                && Crossing.Row == Math.Min(from.Row, to.Row)
                && IsLineCovered(Crossing.Column, from.Column);
        }

        return Orientation == WallOrientation.Vertical
            && Crossing.Column == Math.Min(from.Column, to.Column)
            && IsLineCovered(Crossing.Row, from.Row);
    }

    private static bool IsLineCovered(int start, int line)
    {
        return line >= start && line < start + Length;
    }
}
