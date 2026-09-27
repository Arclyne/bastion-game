using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class ToggleSwitch : Control
{
    private const int TrackWidth = 56;
    private const int TrackHeight = 30;
    private const int KnobInset = 4;

    public string Text { get; set; } = string.Empty;

    public bool IsOn { get; set; }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (IsEnabled && IsHovered && input.HasClicked)
        {
            IsOn = !IsOn;
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        float textY = Bounds.Y + ((Bounds.Height - canvas.Text.GetLineHeight(style)) / 2f);
        canvas.Text.Draw(Text, new Vector2(Bounds.X, MathF.Round(textY)), style);

        var track = new Rectangle(
            Bounds.Right - TrackWidth,
            Bounds.Y + ((Bounds.Height - TrackHeight) / 2),
            TrackWidth,
            TrackHeight);
        int radius = TrackHeight / 2;

        if (IsOn)
        {
            canvas.Shapes.DrawRoundedRectangle(track, radius, Theme.Accent);
        }
        else
        {
            canvas.Shapes.DrawRoundedRectangle(track, radius, Theme.Field);
            Hairline.DrawBorder(
                canvas,
                track,
                BorderStyleFactory.CreateHairline(radius, Theme.CheckBoxBorder));
        }

        int knobSize = TrackHeight - (KnobInset * 2);
        int knobX = IsOn ? track.Right - KnobInset - knobSize : track.X + KnobInset;
        var knob = new Rectangle(knobX, track.Y + KnobInset, knobSize, knobSize);
        canvas.Shapes.DrawRoundedRectangle(knob, knobSize / 2, IsOn ? Theme.Card : Theme.Label);
    }
}
