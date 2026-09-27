using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class PanelBox : Control
{
    public const int Padding = 18;

    private const int TextNudge = 4;

    public string Title { get; set; } = string.Empty;

    public string Footer { get; set; } = string.Empty;

    public bool IsDashed { get; init; }

    public bool IsMuted { get; init; }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        if (IsDashed)
        {
            canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Card);
            Hairline.DrawBorder(
                canvas,
                Bounds,
                BorderStyleFactory.CreateHairline(Theme.FieldCornerRadius, Theme.CheckBoxBorder));
        }
        else
        {
            canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, Theme.Field);
        }

        TextStyle small = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);

        if (!string.IsNullOrEmpty(Title))
        {
            canvas.Text.Draw(Title, new Vector2(Bounds.X + Padding, Bounds.Y + Padding - TextNudge), small);
        }

        if (!string.IsNullOrEmpty(Footer))
        {
            float y = Bounds.Bottom - Padding - canvas.Text.GetLineHeight(small) + TextNudge;
            canvas.Text.Draw(Footer, new Vector2(Bounds.X + Padding, MathF.Round(y)), small);
        }
    }
}
