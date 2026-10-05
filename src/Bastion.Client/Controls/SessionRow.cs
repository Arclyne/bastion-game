using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class SessionRow : Control
{
    private const int HorizontalPadding = 16;
    private const int CloseButtonWidth = 104;
    private const int CloseButtonHeight = 34;
    private const int TextGap = 4;

    private readonly Button _closeButton;

    public SessionRow()
    {
        _closeButton = new Button { Style = ButtonStyle.Secondary };
        _closeButton.Clicked += OnCloseClicked;
    }

    public string Device { get; set; } = string.Empty;

    public string LastUse { get; set; } = string.Empty;

    public string CloseLabel { get; set; } = string.Empty;

    public string CurrentLabel { get; set; } = string.Empty;

    public bool IsCurrent { get; init; }

    public event EventHandler? CloseRequested;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsCurrent)
        {
            _closeButton.Update(input);
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);
        DrawTexts(canvas);

        if (IsCurrent)
        {
            DrawCurrentBadge(canvas);
            return;
        }

        int x = Bounds.Right - HorizontalPadding - CloseButtonWidth;
        int y = Bounds.Y + ((Bounds.Height - CloseButtonHeight) / 2);

        _closeButton.Title = CloseLabel;
        _closeButton.MoveTo(new Rectangle(x, y, CloseButtonWidth, CloseButtonHeight));
        _closeButton.Draw(canvas);
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void DrawTexts(Canvas canvas)
    {
        TextStyle deviceStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        TextStyle lastUseStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);

        float total = canvas.Text.GetLineHeight(deviceStyle) + TextGap + canvas.Text.GetLineHeight(lastUseStyle);
        float y = Bounds.Y + ((Bounds.Height - total) / 2f);
        float x = Bounds.X + HorizontalPadding;

        canvas.Text.Draw(Device, new Vector2(x, MathF.Round(y)), deviceStyle);

        y += canvas.Text.GetLineHeight(deviceStyle) + TextGap;
        canvas.Text.Draw(LastUse, new Vector2(x, MathF.Round(y)), lastUseStyle);
    }

    private void DrawCurrentBadge(Canvas canvas)
    {
        TextStyle style = TextStyleFactory.CreateSmallBold(canvas.Fonts, Theme.Accent);
        float width = canvas.Text.Measure(CurrentLabel, style);
        float x = Bounds.Right - HorizontalPadding - width;
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);

        canvas.Text.Draw(CurrentLabel, new Vector2(MathF.Round(x), MathF.Round(y)), style);
    }
}
