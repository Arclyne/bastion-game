using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Screens.Register;

// CU-02 main flow. Registration only asks for what some flow uses: no first or last names (CU-02 RN-16).
public sealed class RegisterScreen : FormScreen
{
    private const int MaxNicknameLength = 30;
    private const int MaxEmailLength = 254;
    private const int MaxDayLength = 2;
    private const int MaxMonthLength = 2;
    private const int MaxYearLength = 4;

    // A row holds its label, its field and the warning line under it, plus a little air.
    private const int RowAir = 4;
    private const int RegisterRowSpacing = LabelSpace + Theme.FieldHeight + Theme.WarningSpace + RowAir;
    private const int LastRowIndex = 2;

    // The last row prints its warnings into the bottom padding, which therefore cannot be the usual CardPadding.
    private const int BottomPadding = Theme.WarningSpace + RowAir;
    private const int WideCardHeight =
        Theme.CardPadding + LabelSpace + (LastRowIndex * RegisterRowSpacing) + Theme.FieldHeight + BottomPadding;

    private const int AccountRow = 0;
    private const int PasswordRow = 1;
    private const int BirthDateRow = 2;

    private const int DayWidth = 92;
    private const int MonthWidth = 92;
    private const int DateGap = 12;
    private const int DateGapCount = 2;

    private readonly RegisterController _controller;
    private readonly TextField _nicknameField;
    private readonly TextField _emailField;
    private readonly TextField _passwordField;
    private readonly TextField _confirmationField;
    private readonly TextField _dayField;
    private readonly TextField _monthField;
    private readonly TextField _yearField;
    private readonly CheckBox _termsCheckBox;
    private readonly Button _createButton;
    private readonly Button _cancelButton;

    private Task<RegistrationResult>? _pendingRegistration;
    private bool _hasValidated;

    public RegisterScreen(INavigator navigator, RegisterController controller)
        : base(navigator, WideCardWidth, WideCardHeight)
    {
        ArgumentNullException.ThrowIfNull(controller);

        _controller = controller;
        _nicknameField = CreateField(AccountRow, false, MaxNicknameLength);
        _emailField = CreateField(AccountRow, true, MaxEmailLength);
        _passwordField = CreatePasswordField(false);
        _confirmationField = CreatePasswordField(true);
        _dayField = CreateDatePart(ContentX, DayWidth, MaxDayLength);
        _monthField = CreateDatePart(ContentX + DayWidth + DateGap, MonthWidth, MaxMonthLength);
        _yearField = CreateDatePart(
            ContentX + DayWidth + MonthWidth + (DateGap * DateGapCount),
            GetYearWidth(),
            MaxYearLength);
        _termsCheckBox = new CheckBox { Bounds = GetCell(BirthDateRow, true) };
        _createButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();

        _createButton.Clicked += OnCreateAccountClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterField(_nicknameField);
        RegisterField(_emailField);
        RegisterField(_passwordField);
        RegisterField(_confirmationField);
        RegisterField(_dayField);
        RegisterField(_monthField);
        RegisterField(_yearField);
        Register(_termsCheckBox);
        Register(_createButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override int RowPitch => RegisterRowSpacing;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_pendingRegistration is { IsCompleted: true } completed)
        {
            _pendingRegistration = null;
            SetBusy(false);
            HandleResult(completed.GetAwaiter().GetResult().Code);
        }
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.RegisterSubtitle;
    }

    protected override void ApplyTexts()
    {
        _nicknameField.Label = TextCatalog.RegisterNicknameLabel;
        _nicknameField.Placeholder = TextCatalog.RegisterNicknamePlaceholder;
        _emailField.Label = TextCatalog.RegisterEmailLabel;
        _emailField.Placeholder = TextCatalog.RegisterEmailPlaceholder;
        _passwordField.Label = TextCatalog.RegisterPasswordLabel;
        _passwordField.Placeholder = TextCatalog.RegisterPasswordPlaceholder;
        _confirmationField.Label = TextCatalog.RegisterConfirmationLabel;
        _confirmationField.Placeholder = TextCatalog.RegisterConfirmationPlaceholder;
        _dayField.Label = TextCatalog.RegisterBirthDateLabel;
        _dayField.Placeholder = TextCatalog.RegisterDayPlaceholder;
        _monthField.Placeholder = TextCatalog.RegisterMonthPlaceholder;
        _yearField.Placeholder = TextCatalog.RegisterYearPlaceholder;
        _termsCheckBox.Text = TextCatalog.RegisterTermsText;
        _termsCheckBox.LinkText = TextCatalog.RegisterTermsLinkText;
        _createButton.Title = _pendingRegistration is null
            ? TextCatalog.RegisterCreateButton
            : TextCatalog.RegisterCreating;
        _createButton.Subtitle = TextCatalog.RegisterCreateButtonDetail;
        _cancelButton.Title = TextCatalog.RegisterCancelButton;

        if (_hasValidated)
        {
            ShowWarnings(RegisterController.Validate(ReadForm()));
        }
    }

    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        if (_pendingRegistration is not null)
        {
            return;
        }

        _hasValidated = true;
        RegistrationWarnings warnings = RegisterController.Validate(ReadForm());
        ShowWarnings(warnings);
        if (warnings.HasAny)
        {
            Popup.ShowAlert(Navigator, TextCatalog.RegisterCheckTheForm);
            return;
        }

        SetBusy(true);
        _pendingRegistration = _controller.RegisterAsync(ReadForm());
    }

    private void HandleResult(RegistrationResultCode code)
    {
        if (code == RegistrationResultCode.Registered)
        {
            Navigator.GoTo(ScreenId.RegistrationSuccess, _emailField.Text.Trim());
            return;
        }

        RegistrationWarnings? fieldWarnings = RegisterController.GetFieldWarnings(code);
        if (fieldWarnings is not null)
        {
            ShowWarnings(fieldWarnings);
            return;
        }

        Popup.ShowError(Navigator, RegisterController.GetFailureMessage(code));
    }

    private void ShowWarnings(RegistrationWarnings warnings)
    {
        _nicknameField.Warning = warnings.Nickname;
        _emailField.Warning = warnings.Email;
        _passwordField.Warning = warnings.Password;
        _confirmationField.Warning = warnings.Confirmation;

        // Only the day box carries the text, or it would be drawn three times.
        _dayField.Warning = warnings.BirthDate;
        _termsCheckBox.Warning = warnings.Terms;
    }

    private RegistrationForm ReadForm()
    {
        return new RegistrationForm
        {
            Nickname = _nicknameField.Text,
            Email = _emailField.Text,
            Password = _passwordField.Text,
            Confirmation = _confirmationField.Text,
            Day = _dayField.Text,
            Month = _monthField.Text,
            Year = _yearField.Text,
            HasAcceptedTerms = _termsCheckBox.IsChecked,
        };
    }

    private void SetBusy(bool isBusy)
    {
        _createButton.IsEnabled = !isBusy;
        _cancelButton.IsEnabled = !isBusy;
        _createButton.Title = isBusy ? TextCatalog.RegisterCreating : TextCatalog.RegisterCreateButton;
    }

    // CU-02 FA-01: leaving asks first, because what was typed is lost.
    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Popup.Ask(Navigator, new ConfirmRequest
        {
            Body = TextCatalog.RegisterDiscardBody,
            PrimaryLabel = TextCatalog.RegisterDiscardButton,
            SecondaryLabel = TextCatalog.RegisterKeepEditingButton,
            OnConfirm = GoBackToLogin,
        });
    }

    private void GoBackToLogin()
    {
        Navigator.GoBack();
    }

    private int GetYearWidth()
    {
        return ColumnWidth - DayWidth - MonthWidth - (DateGap * DateGapCount);
    }

    private TextField CreateField(int row, bool isRightColumn, int maxLength)
    {
        return new TextField { MaxLength = maxLength, Bounds = GetCell(row, isRightColumn) };
    }

    private TextField CreatePasswordField(bool isRightColumn)
    {
        return new TextField { IsPassword = true, Bounds = GetCell(PasswordRow, isRightColumn) };
    }

    // Three boxes instead of one field: clearer for an eight-year-old player (CON-12), with no format to explain.
    private TextField CreateDatePart(int x, int width, int maxLength)
    {
        return new TextField
        {
            MaxLength = maxLength,
            IsCentered = true,
            Bounds = new Rectangle(x, FirstRowTop + (BirthDateRow * RowPitch), width, Theme.FieldHeight),
        };
    }
}
