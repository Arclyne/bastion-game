using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class TextLine : Control
{
    public string Text { get; set; } = string.Empty;

    public TextLineStyle Style { get; init; } = TextLineStyle.Body;

    public bool IsRightAligned { get; init; }

    public bool IsCentered { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible || string.IsNullOrEmpty(Text))
        {
            return;
        }

        TextStyle style = Style switch
        {
            TextLineStyle.Heading => TextStyleFactory.CreateHeading(canvas.Fonts, Theme.TextDark),
            TextLineStyle.Small => TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder),
            TextLineStyle.Label => TextStyleFactory.CreateLabel(canvas.Fonts, Theme.Label),
            TextLineStyle.Muted => TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder),
            _ => TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark)
        };

        float width = canvas.Text.Measure(Text, style);
        float x = GetLeft(width);
        canvas.Text.Draw(Text, new Vector2(MathF.Round(x), Bounds.Y), style);
    }

    private float GetLeft(float width)
    {
        if (IsRightAligned)
        {
            return Bounds.Right - width;
        }

        if (IsCentered)
        {
            return Bounds.X + ((Bounds.Width - width) / 2f);
        }

        return Bounds.X;
    }
}
