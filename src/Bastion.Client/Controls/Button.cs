using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class Button : Control
{
    private const int ContentPadding = 28;
    private const int TextInset = 12;
    private const int ArrowMargin = 40;
    private const int ArrowHalfWidth = 9;
    private const int ArrowHalfHeight = 7;
    private const int ArrowStroke = 2;
    private const int ArrowHalfStroke = ArrowStroke / 2;
    private const int ArrowHeadOffset = 2;
    private const int SubtitleGap = 2;
    private const float HoverLift = 0.12f;

    private static readonly Color PrimarySubtitle = new(0xFF, 0xD8, 0xCC);

    public string Title { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public ButtonStyle Style { get; init; } = ButtonStyle.Primary;

    public bool HasArrow { get; init; }

    public bool IsCompact { get; init; }

    // Lets a dialog give its primary button the tone color instead of always
    // the orange accent.
    public Color Accent { get; init; } = Theme.Accent;

    public event EventHandler? Clicked;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsEnabled && IsHovered && input.HasClicked)
        {
            OnClicked();
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        switch (Style)
        {
            case ButtonStyle.Primary:
                DrawPrimary(canvas);
                break;

            case ButtonStyle.Link:
                DrawLink(canvas);
                break;

            case ButtonStyle.Outline:
                DrawOutline(canvas);
                break;

            case ButtonStyle.Secondary:
            default:
                DrawSecondary(canvas);
                break;
        }
    }

    private void OnClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }

    private void DrawPrimary(Canvas canvas)
    {
        if (!IsEnabled)
        {
            DrawDisabled(canvas);
            return;
        }

        Color fill = IsHovered ? Color.Lerp(Accent, Color.White, HoverLift) : Accent;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.ButtonCornerRadius, fill);
        DrawContent(canvas, Theme.TextLight, PrimarySubtitle);
    }

    private void DrawSecondary(Canvas canvas)
    {
        Color fill = IsHovered ? Theme.SecondaryButtonHovered : Theme.SecondaryButton;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.ButtonCornerRadius, fill);
        canvas.Shapes.DrawRoundedBorder(
            Bounds,
            BorderStyleFactory.CreateHairline(Theme.ButtonCornerRadius, Theme.SecondaryBorder));
        DrawContent(canvas, Theme.TextLight, Theme.TextMuted);
    }

    private void DrawOutline(Canvas canvas)
    {
        if (!IsEnabled)
        {
            DrawDisabled(canvas);
            return;
        }

        DrawSecondary(canvas);
    }

    private void DrawDisabled(Canvas canvas)
    {
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.ButtonCornerRadius, Theme.Field);
        DrawContent(canvas, Theme.Placeholder, Theme.Placeholder);
    }

    private void DrawLink(Canvas canvas)
    {
        Color color = IsHovered ? Theme.AccentLight : Theme.Accent;
        TextStyle style = TextStyleFactory.CreateSmallBold(canvas.Fonts, color);
        canvas.Text.Draw(Title, new Vector2(Bounds.X, Bounds.Y), style);
    }

    private void DrawContent(Canvas canvas, Color titleColor, Color subtitleColor)
    {
        TextStyle titleStyle = IsCompact
            ? TextStyleFactory.CreateSmallBold(canvas.Fonts, titleColor)
            : TextStyleFactory.CreateBoldBody(canvas.Fonts, titleColor);
        int available = Bounds.Width - (TextInset * 2) - (HasArrow ? ArrowMargin : 0);

        if (canvas.Text.Measure(Title, titleStyle) > available)
        {
            titleStyle = TextStyleFactory.CreateSmallBold(canvas.Fonts, titleColor);
        }

        if (string.IsNullOrEmpty(Subtitle))
        {
            canvas.Text.DrawCentered(Title, Bounds, titleStyle);
            DrawArrowWhenRequested(canvas, titleColor);
            return;
        }

        TextStyle subtitleStyle = TextStyleFactory.CreateSmall(canvas.Fonts, subtitleColor);
        float titleHeight = canvas.Text.GetLineHeight(titleStyle);
        float subtitleHeight = canvas.Text.GetLineHeight(subtitleStyle);
        float top = Bounds.Y + ((Bounds.Height - (titleHeight + subtitleHeight + SubtitleGap)) / 2f);

        canvas.Text.Draw(Title, new Vector2(Bounds.X + ContentPadding, MathF.Round(top)), titleStyle);

        float subtitleTop = MathF.Round(top + titleHeight + SubtitleGap);
        canvas.Text.Draw(Subtitle, new Vector2(Bounds.X + ContentPadding, subtitleTop), subtitleStyle);

        DrawArrowWhenRequested(canvas, titleColor);
    }

    private void DrawArrowWhenRequested(Canvas canvas, Color color)
    {
        if (!HasArrow)
        {
            return;
        }

        DrawArrow(canvas, new Point(Bounds.Right - ArrowMargin, Bounds.Center.Y), color);
    }

    // A right pointing arrow built from one stroke and two diagonals.
    private static void DrawArrow(Canvas canvas, Point center, Color color)
    {
        var stroke = new Rectangle(
            center.X - ArrowHalfWidth,
            center.Y - ArrowHalfStroke,
            ArrowHalfWidth * 2,
            ArrowStroke);
        canvas.Shapes.DrawRectangle(stroke, color);

        for (int step = 0; step < ArrowHalfHeight; step++)
        {
            int x = center.X + ArrowHeadOffset + step;
            int upperY = center.Y - ArrowHalfHeight + step;
            int lowerY = center.Y + ArrowHalfHeight - ArrowStroke - step;
            canvas.Shapes.DrawRectangle(new Rectangle(x, upperY, ArrowStroke, ArrowStroke), color);
            canvas.Shapes.DrawRectangle(new Rectangle(x, lowerY, ArrowStroke, ArrowStroke), color);
        }
    }
}
