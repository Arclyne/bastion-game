using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.PendingVerification;

public sealed class PendingVerificationScreen : FormScreen
{
    public const string DefaultArgument = "";

    private const int NoticeHeight = 48;
    private const int CardHeight = Theme.CardPadding + NoticeHeight + Theme.CardPadding;

    private readonly string _email;
    private readonly TextBlock _notice;
    private readonly Button _acceptButton;
    private readonly Button _resendButton;

    public PendingVerificationScreen(INavigator navigator, string email)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(email);

        _email = email;
        _notice = new TextBlock
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, NoticeHeight)
        };

        _acceptButton = CreatePrimaryButton(false);
        _resendButton = CreateSecondaryButton();
        _acceptButton.Clicked += OnAcceptClicked;
        _resendButton.Clicked += OnResendClicked;

        Register(_notice);
        Register(_acceptButton);
        Register(_resendButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PendingVerificationSubtitle;
    }

    protected override void ApplyTexts()
    {
        _notice.Text = GetNotice();
        _acceptButton.Title = TextCatalog.PendingVerificationAcceptButton;
        _resendButton.Title = TextCatalog.PendingVerificationResendButton;
    }

    private string GetNotice()
    {
        if (string.IsNullOrWhiteSpace(_email))
        {
            return TextCatalog.PendingVerificationNotice;
        }

        return string.Format(
            CultureInfo.CurrentCulture,
            TextCatalog.PendingVerificationNoticeFormat,
            EmailMask.Apply(_email));
    }

    private void OnAcceptClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.Login);
    }

    private void OnResendClicked(object? sender, EventArgs e)
    {
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.PendingVerificationResentBody);
    }
}
