using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Localization;

namespace Bastion.Client.Controls;

// The interface language picker sits in the same corner on every screen that
// offers it, so its geometry and its options are declared once.
public static class LanguagePicker
{
    public const int SpanishIndex = 0;
    public const int EnglishIndex = 1;

    private const int Margin = 24;
    private const int PickerWidth = 210;
    private const int PickerHeight = 40;

    public static DropDown Create()
    {
        return new DropDown
        {
            Options = GetNames(),
            SelectedIndex = Language.IsEnglish ? EnglishIndex : SpanishIndex,
            // Top-right corner: the title is centred and the card starts lower, so no screen places anything there.
            OpensDownward = true,
            Bounds = new Rectangle(
                Theme.WindowWidth - Margin - PickerWidth,
                Margin,
                PickerWidth,
                PickerHeight)
        };
    }

    public static IReadOnlyList<string> GetNames()
    {
        return [TextCatalog.SpanishMexicoLanguageName, TextCatalog.EnglishLanguageName];
    }

    public static void Apply(int selectedIndex)
    {
        Language.Apply(selectedIndex == EnglishIndex ? Language.English : Language.SpanishMexico);
    }
}
