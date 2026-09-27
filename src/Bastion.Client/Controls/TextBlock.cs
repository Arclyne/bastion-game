using System;

namespace Bastion.Client.Controls;

public sealed class TextBlock : Control
{
    public string Text { get; set; } = string.Empty;

    public bool IsSmall { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible || string.IsNullOrEmpty(Text))
        {
            return;
        }

        TextStyle style = IsSmall
            ? TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder)
            : TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        canvas.Text.DrawWrapped(Text, Bounds, style);
    }
}
