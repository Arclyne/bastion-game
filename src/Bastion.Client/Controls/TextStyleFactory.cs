using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public static class TextStyleFactory
{
    public static TextStyle CreateTitle(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Title,
            Color = color,
            Scale = Theme.TitleScale,
            Tracking = Theme.TitleTracking
        };
    }

    public static TextStyle CreateHeading(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Bold,
            Color = color,
            Scale = Theme.HeadingScale
        };
    }

    public static TextStyle CreateLabel(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Bold,
            Color = color,
            Scale = Theme.LabelScale,
            Tracking = Theme.LabelTracking
        };
    }

    public static TextStyle CreateBody(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Regular,
            Color = color,
            Scale = Theme.BodyScale
        };
    }

    public static TextStyle CreateBoldBody(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Bold,
            Color = color,
            Scale = Theme.BodyScale,
            Tracking = Theme.LabelTracking
        };
    }

    public static TextStyle CreateSmall(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Regular,
            Color = color,
            Scale = Theme.SmallScale
        };
    }

    public static TextStyle CreateSmallBold(FontSet fonts, Color color)
    {
        ArgumentNullException.ThrowIfNull(fonts);

        return new TextStyle
        {
            Font = fonts.Bold,
            Color = color,
            Scale = Theme.SmallScale,
            Tracking = Theme.LabelTracking
        };
    }
}
