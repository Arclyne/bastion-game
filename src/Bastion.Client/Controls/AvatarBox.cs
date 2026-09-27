using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

// Placeholder for the avatar and its frame. The catalogue of thirty two icons
// is content, so until it is built this draws the icon number.
public sealed class AvatarBox : Control
{
    private const int FrameThickness = 3;

    public string IconLabel { get; set; } = string.Empty;

    public bool IsSelected { get; set; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        Color fill = IsSelected ? Theme.FieldFocused : Theme.Field;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, fill);

        if (IsSelected || IsHovered)
        {
            var border = new BorderStyle
            {
                CornerRadius = Theme.FieldCornerRadius,
                Color = Theme.Accent,
                Thickness = FrameThickness
            };

            canvas.Shapes.DrawRoundedBorder(Bounds, border);
        }

        TextStyle style = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.Placeholder);
        canvas.Text.DrawCentered(IconLabel, Bounds, style);
    }
}
