namespace Bastion.Client.Rendering;

// What the player is about to do. Nothing is previewed while it is None, which
// is what keeps the board clear when neither a move nor a wall is being made.
public enum BoardAction
{
    None,
    MovePawn,
    PlaceWall
}
