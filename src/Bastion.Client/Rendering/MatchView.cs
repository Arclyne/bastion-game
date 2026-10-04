using System.Collections.Generic;

namespace Bastion.Client.Rendering;

// A plain snapshot: the server decides what is on the board, this only draws it.
public sealed class MatchView
{
    public List<PawnMarker> Pawns { get; } = [];

    public List<WallMarker> Walls { get; } = [];
}
