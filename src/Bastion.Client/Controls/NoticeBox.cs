using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

// Warning the player must read before acting (CU-11, CU-13 RN-01).
// It is not a dialog: it stays on the form.
public sealed class NoticeBox : Control
{
    private const int RuleWidth = 4;
    private const int HorizontalPadding = 16;
    private const int VerticalPadding = 14;

    public string Text { get; set; } = string.Empty;

    // A critical notice announces something that cannot be undone.
    public bool IsCritical { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        Color tone = IsCritical ? Theme.Accent : Theme.Label;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);

        var rule = new Rectangle(Bounds.X, Bounds.Y, RuleWidth, Bounds.Height);
        canvas.Shapes.DrawRoundedRectangle(rule, RuleWidth / 2, tone);

        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        var area = new Rectangle(
            Bounds.X + RuleWidth + HorizontalPadding,
            Bounds.Y + VerticalPadding,
            Bounds.Width - RuleWidth - (HorizontalPadding * 2),
            Bounds.Height - (VerticalPadding * 2));

        canvas.Text.DrawWrapped(Text, area, style);
    }
}
