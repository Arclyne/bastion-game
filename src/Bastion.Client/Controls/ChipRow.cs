using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class ChipRow : Control
{
    private const int TextPadding = 14;
    private const int ChipGap = 8;
    private const int CornerRadius = 18;

    private readonly List<Rectangle> _chipAreas = [];

    public List<Chip> Items { get; } = [];

    public bool IsInteractive { get; init; } = true;

    public event EventHandler<SelectionChangedEventArgs>? ChipChosen;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsInteractive || !IsEnabled || !IsVisible || !input.HasClicked)
        {
            return;
        }

        for (int chipIndex = 0; chipIndex < _chipAreas.Count; chipIndex++)
        {
            if (_chipAreas[chipIndex].Contains(input.MousePosition))
            {
                ChipChosen?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = chipIndex });
                return;
            }
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        _chipAreas.Clear();

        if (!IsVisible)
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextDark);
        int x = Bounds.X;
        int y = Bounds.Y;

        foreach (Chip chip in Items)
        {
            int width = (int)MathF.Ceiling(canvas.Text.Measure(chip.Text, style)) + (TextPadding * 2);

            if (x + width > Bounds.Right && x > Bounds.X)
            {
                x = Bounds.X;
                y += Theme.ChipHeight + ChipGap;
            }

            var area = new Rectangle(x, y, width, Theme.ChipHeight);
            DrawChip(canvas, chip, area);
            _chipAreas.Add(area);
            x += width + ChipGap;
        }
    }

    private static void DrawChip(Canvas canvas, Chip chip, Rectangle area)
    {
        if (chip.IsSelected)
        {
            canvas.Shapes.DrawRoundedRectangle(area, CornerRadius, Theme.Accent);
            canvas.Text.DrawCentered(chip.Text, area, TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextLight));
            return;
        }

        if (chip.IsDashed)
        {
            canvas.Shapes.DrawRoundedRectangle(area, CornerRadius, Theme.Card);
            Hairline.DrawBorder(
                canvas,
                area,
                BorderStyleFactory.CreateHairline(CornerRadius, Theme.CheckBoxBorder));
            canvas.Text.DrawCentered(chip.Text, area, TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder));
            return;
        }

        canvas.Shapes.DrawRoundedRectangle(area, CornerRadius, Theme.Field);
        Color color = chip.IsMuted ? Theme.Placeholder : Theme.TextDark;
        canvas.Text.DrawCentered(chip.Text, area, TextStyleFactory.CreateBoldBody(canvas.Fonts, color));
    }
}
