using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Screens.Login;

// CU-01 main flow. The identifier accepts either the nickname or the email, so it is one field and not two.
public sealed class LoginScreen : FormScreen
{
    private const int CardHeight = 272;
    private const int MaxIdentifierLength = 254;
    private const int IdentifierRow = 0;
    private const int PasswordRow = 1;

    // The link sits under a field that validation can reject, so it clears the warning line.
    private const int LinkExtraGap = 6;
    private const int LinkGap = Theme.WarningSpace + LinkExtraGap;
    private const int LinkHeight = 24;
    private const int GuestButtonGap = 26;

    private readonly LoginController _controller;
    private readonly TextField _identifierField;
    private readonly TextField _passwordField;
    private readonly Button _forgotButton;
    private readonly Button _signInButton;
    private readonly Button _createAccountButton;
    private readonly Button _guestButton;

    private Task<LoginResult>? _pendingLogin;
    private bool _hasValidated;

    public LoginScreen(INavigator navigator, LoginController controller)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(controller);

        _controller = controller;
        _identifierField = new TextField { MaxLength = MaxIdentifierLength, Bounds = GetRow(IdentifierRow) };
        _passwordField = new TextField { IsPassword = true, Bounds = GetRow(PasswordRow) };
        _forgotButton = new Button { Style = ButtonStyle.Link, Bounds = GetForgotBounds() };
        _signInButton = CreatePrimaryButton(true);
        _createAccountButton = CreateSecondaryButton();
        _guestButton = new Button { Style = ButtonStyle.Link, Bounds = GetGuestBounds() };

        _signInButton.Clicked += OnSignInClicked;
        _createAccountButton.Clicked += OnCreateAccountClicked;
        _forgotButton.Clicked += OnForgotClicked;
        _guestButton.Clicked += OnGuestClicked;

        RegisterField(_identifierField);
        RegisterField(_passwordField);
        Register(_forgotButton);
        Register(_signInButton);
        Register(_createAccountButton);
        Register(_guestButton);

        ApplyTexts();
        FocusFirstField();
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_pendingLogin is { IsCompleted: true } completed)
        {
            _pendingLogin = null;
            SetBusy(false);
            HandleResult(completed.GetAwaiter().GetResult());
        }
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LoginSubtitle;
    }

    protected override void ApplyTexts()
    {
        _identifierField.Label = TextCatalog.LoginIdentifierLabel;
        _identifierField.Placeholder = TextCatalog.LoginIdentifierPlaceholder;
        _passwordField.Label = TextCatalog.LoginPasswordLabel;
        _passwordField.Placeholder = TextCatalog.LoginPasswordPlaceholder;
        _forgotButton.Title = TextCatalog.LoginForgotLink;
        _signInButton.Title = _pendingLogin is null ? TextCatalog.LoginSignInButton : TextCatalog.LoginSigningIn;
        _signInButton.Subtitle = TextCatalog.LoginSignInDetail;
        _createAccountButton.Title = TextCatalog.LoginCreateAccountButton;
        _guestButton.Title = TextCatalog.LoginGuestButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnSignInClicked(object? sender, EventArgs e)
    {
        if (_pendingLogin is not null)
        {
            return;
        }

        _hasValidated = true;
        if (!Validate())
        {
            return;
        }

        SetBusy(true);
        _pendingLogin = _controller.LogInAsync(_identifierField.Text, _passwordField.Text);
    }

    private bool Validate()
    {
        LoginWarnings warnings = LoginController.Validate(_identifierField.Text, _passwordField.Text);
        _identifierField.Warning = warnings.Identifier;
        _passwordField.Warning = warnings.Password;
        return !warnings.HasAny;
    }

    private void HandleResult(LoginResult result)
    {
        string message = LoginController.GetMessage(result);
        ScreenId? destination = LoginController.GetDestination(result);
        if (destination == ScreenId.MainMenu)
        {
            Navigator.Restart(ScreenId.MainMenu);
            Popup.ShowNotice(Navigator, message);
            return;
        }

        _passwordField.SetText(string.Empty);
        if (destination is ScreenId screen)
        {
            Navigator.GoTo(screen, result.Email);
            return;
        }

        if (result.Code == LoginResultCode.ServiceUnavailable)
        {
            Popup.ShowError(Navigator, message);
            return;
        }

        Popup.ShowAlert(Navigator, message);
    }

    private void SetBusy(bool isBusy)
    {
        _signInButton.IsEnabled = !isBusy;
        _createAccountButton.IsEnabled = !isBusy;
        _signInButton.Title = isBusy ? TextCatalog.LoginSigningIn : TextCatalog.LoginSignInButton;
    }

    private void OnCreateAccountClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Register);
    }

    private void OnForgotClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ForgotPassword);
    }

    private void OnGuestClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.FirstTime);
    }

    private Rectangle GetForgotBounds()
    {
        return new Rectangle(ContentX, GetRow(PasswordRow).Bottom + LinkGap, ContentWidth, LinkHeight);
    }

    private Rectangle GetGuestBounds()
    {
        return new Rectangle(ContentX, SecondaryButtonBounds.Bottom + GuestButtonGap, ContentWidth, LinkHeight);
    }
}
