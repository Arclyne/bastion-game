using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Validation;

namespace Bastion.Client.Screens.ForgotPassword;

public sealed class ForgotPasswordScreen : FormScreen
{
    private const int MaxEmailLength = 254;
    private const int BodyHeight = 44;
    private const int BlockGap = 24;
    private const int ButtonGap = 40;
    private const int GuestBoxHeight = 76;
    private const int ContentHeight =
        BodyHeight + BlockGap + LabelSpace + Theme.FieldHeight
        + BlockGap + Theme.PanelButtonHeight + ButtonGap + GuestBoxHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + Theme.CardPadding;

    private readonly TextBlock _body;
    private readonly TextField _emailField;
    private readonly Button _sendButton;
    private readonly ActionBox _guestBox;
    private bool _hasValidated;

    public ForgotPasswordScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = PanelNarrowWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        int top = PanelContentTop;
        _body = new TextBlock { Bounds = new Rectangle(ContentX, top, ContentWidth, BodyHeight) };

        int fieldTop = top + BodyHeight + BlockGap + LabelSpace;
        _emailField = new TextField
        {
            MaxLength = MaxEmailLength,
            Bounds = new Rectangle(ContentX, fieldTop, ContentWidth, Theme.FieldHeight)
        };

        int buttonTop = fieldTop + Theme.FieldHeight + BlockGap;
        _sendButton = CreatePrimaryButton(true);
        _sendButton.MoveTo(new Rectangle(ContentX, buttonTop, ContentWidth, Theme.PanelButtonHeight));
        _sendButton.Clicked += OnSendClicked;

        _guestBox = new ActionBox
        {
            IsEnabled = false,
            Bounds = new Rectangle(
                ContentX,
                buttonTop + Theme.PanelButtonHeight + ButtonGap,
                ContentWidth,
                GuestBoxHeight)
        };

        Register(_body);
        RegisterField(_emailField);
        Register(_sendButton);
        Register(_guestBox);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ForgotPasswordSubtitle;
    }

    protected override string GetBackLabel()
    {
        return TextCatalog.ForgotPasswordBackLink;
    }

    protected override void ApplyTexts()
    {
        _body.Text = TextCatalog.ForgotPasswordNotice;
        _emailField.Label = TextCatalog.ForgotPasswordEmailLabel;
        _emailField.Placeholder = TextCatalog.ForgotPasswordEmailPlaceholder;
        _sendButton.Title = TextCatalog.ForgotPasswordSendButton;
        _guestBox.Title = TextCatalog.ForgotPasswordGuestTitle;
        _guestBox.Hint = TextCatalog.ForgotPasswordGuestHint;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnSendClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (Validate())
        {
            Navigator.GoTo(ScreenId.ResetPassword, _emailField.Text.Trim());
        }
    }

    private bool Validate()
    {
        _emailField.Warning = InputRules.IsEmail(_emailField.Text.Trim()) ? null : TextCatalog.RegisterEmailInvalid;

        return !_emailField.HasWarning;
    }
}
