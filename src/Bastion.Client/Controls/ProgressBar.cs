using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class ProgressBar : Control
{
    public float Value { get; set; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        int radius = Bounds.Height / 2;
        canvas.Shapes.DrawRoundedRectangle(Bounds, radius, Theme.Field);

        int width = (int)MathF.Round(Bounds.Width * Math.Clamp(Value, 0f, 1f));

        if (width >= Bounds.Height)
        {
            var fill = new Rectangle(Bounds.X, Bounds.Y, width, Bounds.Height);
            canvas.Shapes.DrawRoundedRectangle(fill, radius, Theme.Accent);
        }
    }
}
