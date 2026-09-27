using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

// Picks one option among a few, drawn as adjacent segments.
public sealed class Selector : Control
{
    private const int LabelOffset = 22;
    private const int ActiveInset = 3;
    private const int CompactCornerRadius = 8;

    public required IReadOnlyList<string> Options { get; set; }

    public string Label { get; set; } = string.Empty;

    public bool IsCompact { get; init; }

    public bool IsOnDarkBackground { get; init; }

    public int SelectedIndex { get; set; }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsHovered || !input.HasClicked)
        {
            return;
        }

        for (int segmentIndex = 0; segmentIndex < Options.Count; segmentIndex++)
        {
            if (GetSegment(segmentIndex).Contains(input.MousePosition) && segmentIndex != SelectedIndex)
            {
                SelectedIndex = segmentIndex;
                OnSelectionChanged(segmentIndex);
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

        DrawLabel(canvas);
        DrawBackground(canvas);
        DrawSegments(canvas);
    }

    private void OnSelectionChanged(int selectedIndex)
    {
        SelectionChanged?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = selectedIndex });
    }

    private Rectangle GetSegment(int index)
    {
        int width = Bounds.Width / Options.Count;
        int x = Bounds.X + (index * width);

        // The last segment absorbs the integer division remainder so the group
        // ends exactly where the control ends.
        int actualWidth = index == Options.Count - 1 ? Bounds.Right - x : width;

        return new Rectangle(x, Bounds.Y, actualWidth, Bounds.Height);
    }

    private int GetCornerRadius()
    {
        return IsCompact ? CompactCornerRadius : Theme.FieldCornerRadius;
    }

    private void DrawLabel(Canvas canvas)
    {
        if (string.IsNullOrEmpty(Label))
        {
            return;
        }

        Color color = HasWarning ? Theme.Accent : Theme.Label;
        TextStyle style = TextStyleFactory.CreateLabel(canvas.Fonts, color);
        canvas.Text.Draw(Label, new Vector2(Bounds.X, Bounds.Y - LabelOffset), style);
    }

    private void DrawBackground(Canvas canvas)
    {
        Color fill = IsOnDarkBackground ? Theme.SecondaryButton : Theme.Field;
        canvas.Shapes.DrawRoundedRectangle(Bounds, GetCornerRadius(), fill);

        if (!IsOnDarkBackground)
        {
            return;
        }

        canvas.Shapes.DrawRoundedBorder(
            Bounds,
            BorderStyleFactory.CreateHairline(GetCornerRadius(), Theme.SecondaryBorder));
    }

    private void DrawSegments(Canvas canvas)
    {
        for (int segmentIndex = 0; segmentIndex < Options.Count; segmentIndex++)
        {
            Rectangle area = GetSegment(segmentIndex);
            bool isActive = segmentIndex == SelectedIndex;

            if (isActive)
            {
                DrawActiveSegment(canvas, area);
            }

            TextStyle style = CreateOptionStyle(canvas.Fonts, isActive);
            canvas.Text.DrawCentered(Options[segmentIndex], area, style);
        }
    }

    private static void DrawActiveSegment(Canvas canvas, Rectangle area)
    {
        var inner = new Rectangle(
            area.X + ActiveInset,
            area.Y + ActiveInset,
            area.Width - (ActiveInset * 2),
            area.Height - (ActiveInset * 2));

        canvas.Shapes.DrawRoundedRectangle(inner, Theme.FieldCornerRadius - ActiveInset, Theme.Accent);
    }

    private TextStyle CreateOptionStyle(FontSet fonts, bool isActive)
    {
        Color color = GetOptionColor(isActive);

        if (IsCompact)
        {
            return TextStyleFactory.CreateSmallBold(fonts, color);
        }

        return TextStyleFactory.CreateLabel(fonts, color);
    }

    private Color GetOptionColor(bool isActive)
    {
        if (isActive)
        {
            return Theme.TextLight;
        }

        if (IsOnDarkBackground)
        {
            return Theme.TextMuted;
        }

        return Theme.Label;
    }
}
