namespace Bastion.Domain;

// Counted from the lower left corner, where column 0 is the column "a".
public readonly record struct BoardPosition(int Column, int Row);
