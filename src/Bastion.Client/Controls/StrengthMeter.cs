using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class StrengthMeter : Control
{
    private const int BarCount = 3;
    private const int BarGap = 8;

    public int Strength { get; set; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        int width = (Bounds.Width - ((BarCount - 1) * BarGap)) / BarCount;
        int radius = Bounds.Height / 2;

        for (int barIndex = 0; barIndex < BarCount; barIndex++)
        {
            var bar = new Rectangle(Bounds.X + (barIndex * (width + BarGap)), Bounds.Y, width, Bounds.Height);

            canvas.Shapes.DrawRoundedRectangle(bar, radius, barIndex < Strength ? Theme.Accent : Theme.Field);
        }
    }
}
