using System.Collections.Generic;

namespace Bastion.Client.Rendering;

// A plain snapshot: the server decides what is on the board, this only draws it.
public sealed class MatchView
{
    public List<PawnMarker> Pawns { get; } = [];

    public List<WallMarker> Walls { get; } = [];

    // What the player is about to put down. None of it is on the board yet, so it
    // is drawn see-through and the server knows nothing about it (CU-21 RN-11).
    public List<PawnMarker> PawnPreviews { get; } = [];

    public WallMarker? WallPreview { get; set; }
}
