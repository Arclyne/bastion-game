using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Controls;

public sealed record TextStyle
{
    public required SpriteFont Font { get; init; }

    public required Color Color { get; init; }

    public float Scale { get; init; } = 1f;

    // Extra space inserted between letters. SpriteFont only offers one spacing
    // value shared by every caller, so tracking is applied per draw call.
    public float Tracking { get; init; }
}
