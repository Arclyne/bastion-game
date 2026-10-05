using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class ValueBox : Control
{
    private const int HorizontalPadding = 16;
    private const int LabelOffset = 22;

    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        if (!string.IsNullOrEmpty(Label))
        {
            TextStyle labelStyle = TextStyleFactory.CreateLabel(canvas.Fonts, Theme.Label);
            canvas.Text.Draw(Label, new Vector2(Bounds.X, Bounds.Y - LabelOffset), labelStyle);
        }

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);

        TextStyle valueStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder);
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(valueStyle)) / 2f);
        canvas.Text.Draw(Value, new Vector2(Bounds.X + HorizontalPadding, MathF.Round(y)), valueStyle);
    }
}
