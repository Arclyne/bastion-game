namespace Bastion.Domain;

// The crossing is named by the cell at its lower left corner, as the server
// stores it.
public readonly record struct Wall(BoardPosition Crossing, WallOrientation Orientation);
