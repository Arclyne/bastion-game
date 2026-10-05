using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Versus;

public sealed class VersusScreen : FormScreen
{
    private const int AvatarSize = 72;
    private const int RowTop = 8;
    private const int NameGap = 6;
    private const int NameHeight = 24;
    private const int EloHeight = 20;
    private const int VersusWidth = 60;
    private const int SettingsGap = 20;
    private const int SettingsHeight = 22;

    private const int SettingsTop = RowTop + AvatarSize + NameGap + NameHeight + NameGap + EloHeight + SettingsGap;
    private const int CardHeight = Theme.CardPadding + SettingsTop + SettingsHeight + Theme.CardPadding;

    private readonly Avatar _leftAvatar;
    private readonly TextLine _leftName;
    private readonly TextLine _leftElo;
    private readonly TextLine _versusLabel;
    private readonly Avatar _rightAvatar;
    private readonly TextLine _rightName;
    private readonly TextLine _rightElo;
    private readonly TextLine _settings;
    private readonly Button _startButton;

    public VersusScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding + RowTop;
        int sideWidth = (ContentWidth - VersusWidth) / 2;
        int leftX = ContentX;
        int rightX = ContentX + sideWidth + VersusWidth;
        int nameTop = top + AvatarSize + NameGap;
        int eloTop = nameTop + NameHeight + NameGap;

        _leftAvatar = new Avatar
        {
            Bounds = new Rectangle(leftX + ((sideWidth - AvatarSize) / 2), top, AvatarSize, AvatarSize)
        };
        _leftName = new TextLine
        {
            IsCentered = true,
            Bounds = new Rectangle(leftX, nameTop, sideWidth, NameHeight)
        };
        _leftElo = new TextLine
        {
            Style = TextLineStyle.Small,
            IsCentered = true,
            Bounds = new Rectangle(leftX, eloTop, sideWidth, EloHeight)
        };

        _versusLabel = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(leftX + sideWidth, top + ((AvatarSize - NameHeight) / 2), VersusWidth, NameHeight)
        };

        _rightAvatar = new Avatar
        {
            Bounds = new Rectangle(rightX + ((sideWidth - AvatarSize) / 2), top, AvatarSize, AvatarSize)
        };
        _rightName = new TextLine
        {
            IsCentered = true,
            Bounds = new Rectangle(rightX, nameTop, sideWidth, NameHeight)
        };
        _rightElo = new TextLine
        {
            Style = TextLineStyle.Small,
            IsCentered = true,
            Bounds = new Rectangle(rightX, eloTop, sideWidth, EloHeight)
        };

        _settings = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding + SettingsTop, ContentWidth, SettingsHeight)
        };

        _startButton = CreatePrimaryButton(true);
        _startButton.Clicked += OnStartClicked;

        Register(_leftAvatar);
        Register(_leftName);
        Register(_leftElo);
        Register(_versusLabel);
        Register(_rightAvatar);
        Register(_rightName);
        Register(_rightElo);
        Register(_settings);
        Register(_startButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.VersusScreenSubtitle;
    }

    protected override void ApplyTexts()
    {
        _leftAvatar.Text = TextCatalog.AvatarPlaceholder;
        _versusLabel.Text = TextCatalog.VersusScreenVersusLabel;
        _rightAvatar.Text = TextCatalog.AvatarPlaceholder;
        _startButton.Title = TextCatalog.VersusScreenStartButton;
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Match);
    }
}
