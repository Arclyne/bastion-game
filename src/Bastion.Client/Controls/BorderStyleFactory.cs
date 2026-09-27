using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public static class BorderStyleFactory
{
    private const int ThickBorder = 2;

    public static BorderStyle CreateHairline(int cornerRadius, Color color)
    {
        return new BorderStyle
        {
            CornerRadius = cornerRadius,
            Color = color
        };
    }

    public static BorderStyle CreateThick(int cornerRadius, Color color)
    {
        return new BorderStyle
        {
            CornerRadius = cornerRadius,
            Color = color,
            Thickness = ThickBorder
        };
    }
}
