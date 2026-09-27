using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Controls;

public sealed class FontSet
{
    public required SpriteFont Regular { get; init; }

    public required SpriteFont Bold { get; init; }

    public required SpriteFont Title { get; init; }
}
