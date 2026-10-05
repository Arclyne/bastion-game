using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class DataRow : Control
{
    private const int HorizontalPadding = 16;
    private const int BadgeWidth = 44;
    private const int TextGap = 4;

    public string Badge { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public ValueTone Tone { get; set; } = ValueTone.Neutral;

    public bool IsHighlighted { get; init; }

    public bool IsPodium { get; init; }

    public event EventHandler? Clicked;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsEnabled && IsHovered && input.HasClicked)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        Color fill = IsHighlighted ? Theme.FieldFocused : Theme.Field;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, fill);

        if (IsHighlighted)
        {
            canvas.Shapes.DrawRoundedBorder(
                Bounds,
                BorderStyleFactory.CreateThick(Theme.FieldCornerRadius, Theme.Accent));
        }

        int textLeft = DrawBadge(canvas);
        DrawTexts(canvas, textLeft);
        DrawValue(canvas);
    }

    private int DrawBadge(Canvas canvas)
    {
        int left = Bounds.X + HorizontalPadding;

        if (string.IsNullOrEmpty(Badge))
        {
            return left;
        }

        Color badgeColor = IsPodium ? Theme.Accent : Theme.Placeholder;
        TextStyle style = TextStyleFactory.CreateBoldBody(canvas.Fonts, badgeColor);
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Badge, new Vector2(left, MathF.Round(y)), style);

        return left + BadgeWidth;
    }

    private void DrawTexts(Canvas canvas, int left)
    {
        TextStyle titleStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);

        if (string.IsNullOrEmpty(Subtitle))
        {
            float middle = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(titleStyle)) / 2f);
            canvas.Text.Draw(Title, new Vector2(left, MathF.Round(middle)), titleStyle);
            return;
        }

        TextStyle subtitleStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        float total = canvas.Text.GetLineHeight(titleStyle) + TextGap + canvas.Text.GetLineHeight(subtitleStyle);
        float y = Bounds.Y + ((Bounds.Height - total) / 2f);

        canvas.Text.Draw(Title, new Vector2(left, MathF.Round(y)), titleStyle);
        y += canvas.Text.GetLineHeight(titleStyle) + TextGap;
        canvas.Text.Draw(Subtitle, new Vector2(left, MathF.Round(y)), subtitleStyle);
    }

    private void DrawValue(Canvas canvas)
    {
        if (string.IsNullOrEmpty(Value))
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateBoldBody(canvas.Fonts, GetValueColor());
        float width = canvas.Text.Measure(Value, style);
        float x = Bounds.Right - HorizontalPadding - width;
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);

        canvas.Text.Draw(Value, new Vector2(MathF.Round(x), MathF.Round(y)), style);
    }

    private Color GetValueColor()
    {
        if (Tone == ValueTone.Positive)
        {
            return Theme.Positive;
        }

        if (Tone == ValueTone.Negative)
        {
            return Theme.Accent;
        }

        return Theme.TextDark;
    }
}
