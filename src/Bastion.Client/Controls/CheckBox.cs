using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class CheckBox : Control
{
    private const int BoxSize = 22;
    private const int TextGap = 12;
    private const int CheckStrokeSize = 2;
    private const int CheckMarkLeft = 5;
    private const int CheckMarkBaseline = 11;
    private const int CheckShortArmSteps = 4;
    private const int CheckLongArmSteps = 7;
    private const int CheckLongArmOffset = CheckShortArmSteps - 1;

    public bool IsChecked { get; set; }

    public string Text { get; set; } = string.Empty;

    public string LinkText { get; set; } = string.Empty;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsHovered && input.HasClicked)
        {
            IsChecked = !IsChecked;
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        Rectangle box = GetBox();
        DrawBox(canvas, box);
        DrawText(canvas, box);
        DrawWarning(canvas, box);
    }

    private Rectangle GetBox()
    {
        return new Rectangle(Bounds.X, Bounds.Y + ((Bounds.Height - BoxSize) / 2), BoxSize, BoxSize);
    }

    private void DrawBox(Canvas canvas, Rectangle box)
    {
        if (IsChecked)
        {
            canvas.Shapes.DrawRoundedRectangle(box, Theme.CheckBoxCornerRadius, Theme.Accent);
            DrawCheckMark(canvas, box);
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(box, Theme.CheckBoxCornerRadius, Theme.Card);

        Color borderColor = HasWarning ? Theme.Accent : Theme.CheckBoxBorder;
        BorderStyle border = BorderStyleFactory.CreateThick(Theme.CheckBoxCornerRadius, borderColor);
        canvas.Shapes.DrawRoundedBorder(box, border);
    }

    private void DrawText(Canvas canvas, Rectangle box)
    {
        TextStyle plainStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.Label);
        float x = box.Right + TextGap;
        float y = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(plainStyle)) / 2f);
        string plain = GetPlainPart();

        canvas.Text.Draw(plain, new Vector2(x, MathF.Round(y)), plainStyle);

        if (plain.Length == Text.Length)
        {
            return;
        }

        TextStyle linkStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.Accent);
        linkStyle = linkStyle with { Font = canvas.Fonts.Bold };
        float offset = canvas.Text.Measure(plain, plainStyle);

        canvas.Text.Draw(LinkText, new Vector2(x + offset, MathF.Round(y)), linkStyle);
    }

    private string GetPlainPart()
    {
        if (string.IsNullOrEmpty(LinkText) || !Text.EndsWith(LinkText, StringComparison.Ordinal))
        {
            return Text;
        }

        return Text[..^LinkText.Length];
    }

    private void DrawWarning(Canvas canvas, Rectangle box)
    {
        if (!HasWarning)
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Accent);
        var position = new Vector2(box.Right + TextGap, Bounds.Bottom + Theme.WarningOffset);
        canvas.Text.Draw(Warning ?? string.Empty, position, style);
    }

    private static void DrawCheckMark(Canvas canvas, Rectangle box)
    {
        int x = box.X + CheckMarkLeft;
        int y = box.Y + CheckMarkBaseline;

        for (int step = 0; step < CheckShortArmSteps; step++)
        {
            var stroke = new Rectangle(x + step, y + step, CheckStrokeSize, CheckStrokeSize);
            canvas.Shapes.DrawRectangle(stroke, Theme.TextLight);
        }

        for (int step = 0; step < CheckLongArmSteps; step++)
        {
            var stroke = new Rectangle(
                x + CheckLongArmOffset + step,
                y + CheckLongArmOffset - step,
                CheckStrokeSize,
                CheckStrokeSize);
            canvas.Shapes.DrawRectangle(stroke, Theme.TextLight);
        }
    }
}
