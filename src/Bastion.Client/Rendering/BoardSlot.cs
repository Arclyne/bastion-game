namespace Bastion.Client.Rendering;

// Counted from the lower left corner, where column 0 is the column "a" and row 0
// is the row "1". It names a cell for a pawn and a crossing for a wall.
public readonly record struct BoardSlot(int Column, int Row);
