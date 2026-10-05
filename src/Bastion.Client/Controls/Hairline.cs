using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public static class Hairline
{
    private const int Thickness = 1;

    public static void DrawHorizontal(Canvas canvas, Rectangle line, Color color)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        canvas.Shapes.DrawRectangle(new Rectangle(line.X, line.Y, line.Width, Thickness), color);
    }

    public static void DrawBorder(Canvas canvas, Rectangle bounds, BorderStyle style)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(style);

        canvas.Shapes.DrawRoundedBorder(bounds, style);
    }
}
