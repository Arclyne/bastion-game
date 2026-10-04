using Microsoft.Xna.Framework;

namespace Bastion.Client.Rendering;

public sealed record WallMarker(BoardSlot Groove, WallOrientation Orientation, Color Tint);
