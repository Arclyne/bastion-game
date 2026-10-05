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
}
