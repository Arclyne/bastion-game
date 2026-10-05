namespace Bastion.Domain;

// Where a pawn stands and the side it is trying to reach. The goal is what tells
// one player from another and never changes during a match, so it travels with
// the pawn instead of being worked out from where the pawn started.
public readonly record struct Pawn(BoardPosition Cell, BoardSide Goal);
