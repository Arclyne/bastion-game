using Microsoft.Xna.Framework;
using Bastion.Domain;

namespace Bastion.Client.Rendering;

// The pawn model ships white so that the player colour is the only thing that
// tells two pawns apart, as the asset notes ask.
public sealed record PawnMarker(BoardPosition Cell, Color Tint);
