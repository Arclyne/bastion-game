using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Title;

public sealed class TitleScreen : FormScreen
{
    private const int IntroHeight = 84;
    private const int CardHeight = Theme.CardPadding + IntroHeight + Theme.CardPadding;

    private readonly TextBlock _intro;
    private readonly Button _signInButton;

    public TitleScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _intro = new TextBlock
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, IntroHeight),
        };
        _signInButton = CreatePrimaryButton(true);
        _signInButton.Clicked += OnSignInClicked;

        Register(_intro);
        Register(_signInButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MainScreenSubtitle;
    }

    protected override void ApplyTexts()
    {
        _intro.Text = TextCatalog.MainScreenIntro;
        _signInButton.Title = TextCatalog.LoginSignInButton;
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Login);
    }
}
