using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class StatTile : Control
{
    private const int HorizontalPadding = 16;
    private const int TopPadding = 14;
    private const int TextGap = 4;
    private const float ValueScale = 1.5f;

    public string Value { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);

        var valueStyle = new TextStyle { Font = canvas.Fonts.Bold, Color = Theme.TextDark, Scale = ValueScale };
        TextStyle captionStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Label);

        float x = Bounds.X + HorizontalPadding;
        float y = Bounds.Y + TopPadding;
        canvas.Text.Draw(Value, new Vector2(x, y), valueStyle);

        y += canvas.Text.GetLineHeight(valueStyle) + TextGap;
        canvas.Text.Draw(Caption, new Vector2(x, MathF.Round(y)), captionStyle);
    }
}
