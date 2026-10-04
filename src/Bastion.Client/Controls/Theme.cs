using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

// Colors are sampled from the high fidelity login prototype so both access
// screens read as one product instead of two approximations.
public static class Theme
{
    public const int WindowWidth = 1280;
    public const int WindowHeight = 720;

    public const int CardCornerRadius = 24;
    public const int FieldCornerRadius = 12;
    public const int ButtonCornerRadius = 14;
    public const int CheckBoxCornerRadius = 6;

    public const int FieldHeight = 46;
    public const int PrimaryButtonHeight = 60;
    public const int SecondaryButtonHeight = 52;
    public const int CardPadding = 32;
    public const int PanelTop = 24;
    public const int PanelCardWidth = 1184;
    public const int PanelMargin = 24;
    public const int PanelButtonWidth = 260;
    public const int PanelButtonHeight = 52;
    public const int PanelBackHeight = 22;
    public const int PanelTitleHeight = 34;
    public const int PanelGap = 14;
    public const int ChipHeight = 36;
    public const int SmallButtonHeight = 40;

    // Validation prints at most one line under a field. The space is reserved
    // whether or not it is used, so showing a warning never pushes the rest of
    // the form down, and layouts leave at least WarningSpace under any field
    // that can be rejected.
    public const int WarningOffset = 5;

    // Measured, not guessed: the small style is 17.25 px tall at Arial 15, so
    // a warning reaches WarningOffset + 17.25 below its field.
    public const int WarningSpace = 22;
    public const int DialogWidth = 560;
    public const int DialogButtonHeight = 46;
    public const int DialogToneBarHeight = 6;

    public const float TitleScale = 1.0f;
    public const float BodyScale = 1.0f;
    public const float LabelScale = 0.8f;
    public const float SmallScale = 0.75f;
    public const float HeadingScale = 1.4f;

    public const float TitleTracking = 6f;
    public const float LabelTracking = 1.4f;

    public static readonly Color Background = new(0x13, 0x11, 0x10);
    public static readonly Color BackgroundOrnament = new(0x2A, 0x27, 0x24);
    public static readonly Color Card = new(0xF1, 0xEF, 0xE9);
    public static readonly Color Field = new(0xE8, 0xE4, 0xDA);
    public static readonly Color FieldFocused = new(0xDE, 0xD9, 0xCC);

    // Laid over the board while it is not your turn (CU-20, CU-21).
    public static readonly Color BoardVeil = new(0x13, 0x11, 0x10, 0x8C);

    // Player colours proposed by the model notes, so a pawn is told apart by its
    // colour and not only by its shape.
    public static readonly Color FirstPlayer = new(0xD8, 0x5A, 0x30);
    public static readonly Color SecondPlayer = new(0x1D, 0x9E, 0x75);

    public static readonly Color Accent = new(0xEC, 0x4B, 0x22);
    public static readonly Color AccentLight = new(0xEE, 0x61, 0x3D);

    public static readonly Color TextLight = new(0xF7, 0xF5, 0xF1);
    public static readonly Color TextMuted = new(0x8F, 0x88, 0x80);
    public static readonly Color TextDark = new(0x2A, 0x27, 0x23);
    public static readonly Color Label = new(0x6B, 0x65, 0x5C);
    public static readonly Color Placeholder = new(0xA2, 0x9B, 0x8F);

    public static readonly Color SecondaryButton = new(0x1C, 0x18, 0x15);
    public static readonly Color SecondaryButtonHovered = new(0x26, 0x21, 0x1D);
    public static readonly Color SecondaryBorder = new(0x37, 0x31, 0x2A);
    public static readonly Color CheckBoxBorder = new(0xB9, 0xB2, 0xA5);

    // The only color not sampled from the prototype, which has no green.
    public static readonly Color Positive = new(0x3F, 0x9E, 0x63);

    public static readonly Color Backdrop = new(0x00, 0x00, 0x00);
}
