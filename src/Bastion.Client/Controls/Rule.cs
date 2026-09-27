using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class Rule : Control
{
    private const int Thickness = 1;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (IsVisible)
        {
            Hairline.DrawHorizontal(
                canvas,
                new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Thickness),
                Theme.CheckBoxBorder);
        }
    }
}
