using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Controls;

public sealed record TextStyle
{
    public required SpriteFont Font { get; init; }

    public required Color Color { get; init; }

    public float Scale { get; init; } = 1f;

    public float Tracking { get; init; }
}
