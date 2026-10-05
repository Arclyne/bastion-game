using Microsoft.Xna.Framework;
using Bastion.Domain;

namespace Bastion.Client.Rendering;

public sealed record WallMarker(BoardPosition Groove, WallOrientation Orientation, Color Tint);
