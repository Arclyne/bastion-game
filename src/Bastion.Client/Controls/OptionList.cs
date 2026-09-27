using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

// Picks one option among several, stacked. Selector lays its options side by
// side, which does not fit the five report reasons of CU-42.
public sealed class OptionList : Control
{
    private const int OptionHeight = 44;
    private const int OptionGap = 8;
    private const int MarkerSize = 18;
    private const int MarkerMargin = 14;
    private const int TextMargin = 44;
    private const int MarkerInset = 5;

    public required IReadOnlyList<string> Options { get; set; }

    public int SelectedIndex { get; set; }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public static int GetRequiredHeight(int optionCount)
    {
        return (optionCount * (OptionHeight + OptionGap)) - OptionGap;
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsHovered || !input.HasClicked)
        {
            return;
        }

        for (int optionIndex = 0; optionIndex < Options.Count; optionIndex++)
        {
            if (GetOption(optionIndex).Contains(input.MousePosition) && optionIndex != SelectedIndex)
            {
                SelectedIndex = optionIndex;
                OnSelectionChanged(optionIndex);
                return;
            }
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        for (int optionIndex = 0; optionIndex < Options.Count; optionIndex++)
        {
            DrawOption(canvas, optionIndex);
        }
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = selectedIndex });
    }

    private Rectangle GetOption(int index)
    {
        int top = Bounds.Y + (index * (OptionHeight + OptionGap));

        return new Rectangle(Bounds.X, top, Bounds.Width, OptionHeight);
    }

    private void DrawOption(Canvas canvas, int index)
    {
        Rectangle area = GetOption(index);
        bool isSelected = index == SelectedIndex;

        canvas.Shapes.DrawRoundedRectangle(area, Theme.FieldCornerRadius, Theme.Field);

        if (isSelected)
        {
            canvas.Shapes.DrawRoundedBorder(
                area,
                BorderStyleFactory.CreateThick(Theme.FieldCornerRadius, Theme.Accent));
        }

        DrawMarker(canvas, area, isSelected);

        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        float y = area.Y + ((area.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Options[index], new Vector2(area.X + TextMargin, MathF.Round(y)), style);
    }

    private static void DrawMarker(Canvas canvas, Rectangle area, bool isSelected)
    {
        var marker = new Rectangle(
            area.X + MarkerMargin,
            area.Y + ((area.Height - MarkerSize) / 2),
            MarkerSize,
            MarkerSize);

        canvas.Shapes.DrawRoundedRectangle(marker, MarkerSize / 2, Theme.Card);
        canvas.Shapes.DrawRoundedBorder(
            marker,
            BorderStyleFactory.CreateThick(MarkerSize / 2, isSelected ? Theme.Accent : Theme.CheckBoxBorder));

        if (!isSelected)
        {
            return;
        }

        var dot = new Rectangle(
            marker.X + MarkerInset,
            marker.Y + MarkerInset,
            MarkerSize - (MarkerInset * 2),
            MarkerSize - (MarkerInset * 2));

        canvas.Shapes.DrawRoundedRectangle(dot, (MarkerSize - (MarkerInset * 2)) / 2, Theme.Accent);
    }
}
