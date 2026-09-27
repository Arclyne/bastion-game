using System;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Validation;

namespace Bastion.Client.Screens.ChangePassword;

public sealed class ChangePasswordScreen : FormScreen
{
    private const int Rows = 3;
    private const int ButtonsGap = 24;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + LabelSpace + ((Rows - 1) * RowSpacing) + Theme.FieldHeight + Theme.WarningSpace
        + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly TextField _currentField;
    private readonly TextField _newField;
    private readonly TextField _confirmationField;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public ChangePasswordScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = PanelNarrowWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        _currentField = new TextField { IsPassword = true, Bounds = GetRow(0) };
        _newField = new TextField { IsPassword = true, Bounds = GetRow(1) };
        _confirmationField = new TextField { IsPassword = true, Bounds = GetRow(2) };
        _saveButton = CreatePrimaryButton(true);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_currentField);
        RegisterField(_newField);
        RegisterField(_confirmationField);
        Register(_saveButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangePasswordSubtitle;
    }

    protected override void ApplyTexts()
    {
        _currentField.Label = TextCatalog.ChangePasswordCurrentLabel;
        _currentField.Placeholder = TextCatalog.ChangePasswordCurrentPlaceholder;
        _newField.Label = TextCatalog.ChangePasswordNewLabel;
        _newField.Placeholder = TextCatalog.ChangePasswordNewPlaceholder;
        _confirmationField.Label = TextCatalog.ChangePasswordConfirmLabel;
        _confirmationField.Placeholder = TextCatalog.ChangePasswordConfirmPlaceholder;
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
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.ChangePasswordDoneBody);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    // Only the server knows the current password, so the client just checks
    // that one was typed.
    private bool Validate()
    {
        _currentField.Warning = _currentField.Text.Length > 0 ? null : TextCatalog.LoginPasswordRequired;
        _newField.Warning = InputRules.MeetsPasswordPolicy(_newField.Text) ? null : TextCatalog.PasswordPolicyWarning;
        _confirmationField.Warning = _confirmationField.Text == _newField.Text
            ? null
            : TextCatalog.RegisterConfirmationMismatch;

        return !_currentField.HasWarning && !_newField.HasWarning && !_confirmationField.HasWarning;
    }
}
