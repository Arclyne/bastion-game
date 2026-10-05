namespace Bastion.Domain;

// The side of the board a pawn must reach to win, which is the one opposite the
// one it starts on. Four sides because the four player mode starts two of them
// on the left and the right.
public enum BoardSide
{
    Bottom,
    Top,
    Left,
    Right
}
