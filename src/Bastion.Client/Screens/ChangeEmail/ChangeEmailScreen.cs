using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Validation;

namespace Bastion.Client.Screens.ChangeEmail;

public sealed class ChangeEmailScreen : FormScreen
{
    private const int MaxEmailLength = 254;
    private const int NoticeHeight = 64;
    private const int NoticeGap = 26;
    private const int ButtonsGap = 24;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + LabelSpace + RowSpacing + Theme.FieldHeight + NoticeGap + NoticeHeight
        + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly TextField _newEmailField;
    private readonly TextField _passwordField;
    private readonly NoticeBox _notice;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public ChangeEmailScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = PanelNarrowWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        _newEmailField = new TextField { MaxLength = MaxEmailLength, Bounds = GetRow(0) };
        _passwordField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _notice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, GetRow(1).Bottom + NoticeGap, ContentWidth, NoticeHeight)
        };
        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_newEmailField);
        RegisterField(_passwordField);
        Register(_notice);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangeEmailSubtitle;
    }

    protected override void ApplyTexts()
    {
        _newEmailField.Label = TextCatalog.ChangeEmailNewLabel;
        _newEmailField.Placeholder = TextCatalog.ChangeEmailNewPlaceholder;
        _passwordField.Label = TextCatalog.ChangeEmailPasswordLabel;
        _passwordField.Placeholder = TextCatalog.ChangeEmailPasswordPlaceholder;
        _notice.Text = TextCatalog.ChangeEmailNotice;
        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        Navigator.ReturnTo(ScreenId.AccountSettings);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.ChangeEmailDoneBody);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Whether the address is taken and whether the password is right are
    // answered by the server; the client only rejects what is plainly wrong.
    private bool Validate()
    {
        _newEmailField.Warning = InputRules.IsEmail(_newEmailField.Text.Trim())
            ? null
            : TextCatalog.RegisterEmailInvalid;
        _passwordField.Warning = _passwordField.Text.Length > 0 ? null : TextCatalog.LoginPasswordRequired;

        return !_newEmailField.HasWarning && !_passwordField.HasWarning;
    }
}
