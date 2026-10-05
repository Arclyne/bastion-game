namespace Bastion.Domain;

// What CU-21 says the player is told when a wall cannot be let go of: it
// overlaps, it crosses another one, or it would shut a player in (FA-02).
public enum WallPlacementResult
{
    Allowed,
    OutsideTheBoard,
    Overlaps,
    Crosses,
    ShutsAPlayerIn
}
