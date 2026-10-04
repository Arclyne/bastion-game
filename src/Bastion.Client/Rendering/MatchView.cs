using System.Collections.Generic;

namespace Bastion.Client.Rendering;

// A plain snapshot: the server decides what is on the board, this only draws it.
public sealed class MatchView
{
    public List<PawnMarker> Pawns { get; } = [];

    public List<WallMarker> Walls { get; } = [];

    // CU-20 and CU-21 dim the board while it is not your turn.
    public bool IsDimmed { get; set; }

    public void Clear()
    {
        Pawns.Clear();
        Walls.Clear();
    }
}
