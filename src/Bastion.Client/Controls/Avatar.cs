using System;

namespace Bastion.Client.Controls;

public sealed class Avatar : Control
{
    // From this width on the initials fit in the body size; smaller avatars use the small one.
    private const int LargeAvatarWidth = 64;

    public string Text { get; set; } = string.Empty;

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

        int radius = Math.Min(Bounds.Width, Bounds.Height) / 2;
        canvas.Shapes.DrawRoundedRectangle(Bounds, radius, Theme.Field);
        Hairline.DrawBorder(canvas, Bounds, BorderStyleFactory.CreateHairline(radius, Theme.CheckBoxBorder));

        TextStyle style = Bounds.Width >= LargeAvatarWidth
            ? TextStyleFactory.CreateBody(canvas.Fonts, Theme.Placeholder)
            : TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        canvas.Text.DrawCentered(Text, Bounds, style);
    }
}
