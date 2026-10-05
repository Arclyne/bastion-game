using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.RegistrationSuccess;

public sealed class RegistrationSuccessScreen : FormScreen
{
    private const int NoticeHeight = 96;
    private const int NoticeGap = 26;
    private const int CardHeight =
        Theme.CardPadding + NoticeHeight + NoticeGap + LabelSpace + Theme.FieldHeight + Theme.CardPadding;

    private readonly NoticeBox _notice;
    private readonly ValueBox _emailBox;
    private readonly Button _signInButton;
    private readonly string _email;

    public RegistrationSuccessScreen(INavigator navigator, string email)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(email);

        _email = email;
        _notice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight),
        };

        int emailTop = _notice.Bounds.Bottom + NoticeGap + LabelSpace;
        _emailBox = new ValueBox { Bounds = new Rectangle(ContentX, emailTop, ContentWidth, Theme.FieldHeight) };
        _signInButton = CreatePrimaryButton(true);
        _signInButton.Clicked += OnSignInClicked;

        Register(_notice);
        Register(_emailBox);
        Register(_signInButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.RegistrationSuccessSubtitle;
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.RegistrationSuccessNotice;
        _emailBox.Label = TextCatalog.RegistrationSuccessEmailLabel;
        _emailBox.Value = EmailMask.Apply(_email);
        _signInButton.Title = TextCatalog.RegistrationSuccessSignInButton;
        _signInButton.Subtitle = TextCatalog.RegistrationSuccessDetail;
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.Login);
    }
}
