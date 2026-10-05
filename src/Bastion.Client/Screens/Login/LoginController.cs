using System;
using System.Globalization;
using System.Threading.Tasks;
using Bastion.Client.Localization;
using Bastion.Client.Networking;
using Bastion.Client.Session;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Screens.Login;

public sealed class LoginController
{
    private readonly IAccountClient _accounts;
    private readonly SessionContext _session;

    public LoginController(IAccountClient accounts, SessionContext session)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        ArgumentNullException.ThrowIfNull(session);

        _accounts = accounts;
        _session = session;
    }

    public static LoginWarnings Validate(string identifier, string password)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        ArgumentNullException.ThrowIfNull(password);

        string? identifierWarning = string.IsNullOrWhiteSpace(identifier) ? TextCatalog.LoginIdentifierRequired : null;
        string? passwordWarning = password.Length == 0 ? TextCatalog.LoginPasswordRequired : null;
        return new LoginWarnings(identifierWarning, passwordWarning);
    }

    public async Task<LoginResult> LogInAsync(string identifier, string password)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        ArgumentNullException.ThrowIfNull(password);

        var request = new LoginRequest
        {
            Identifier = identifier.Trim(),
            Password = password,
            DeviceFingerprint = DeviceIdentity.GetFingerprint(),
            ClientVersion = DeviceIdentity.GetClientVersion(),
        };
        LoginResult result = await _accounts.LogInAsync(request);
        if (result.Code == LoginResultCode.Success)
        {
            _session.Start(result);
        }

        return result;
    }

    public static ScreenId? GetDestination(LoginResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        switch (result.Code)
        {
            case LoginResultCode.Success:
                return ScreenId.MainMenu;
            case LoginResultCode.AccountPending:
                return ScreenId.PendingVerification;
            case LoginResultCode.AccountSuspended:
            case LoginResultCode.AccountBanned:
                return ScreenId.BannedAccount;
            default:
                return null;
        }
    }

    public static string GetMessage(LoginResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        switch (result.Code)
        {
            case LoginResultCode.Success:
                return string.Format(CultureInfo.CurrentCulture, TextCatalog.LoginSuccess, result.Nickname);
            case LoginResultCode.AccountLocked:
                return string.Format(CultureInfo.CurrentCulture, TextCatalog.LoginAccountLocked, result.LockMinutes);
            case LoginResultCode.AccountPending:
                return TextCatalog.LoginAccountPending;
            case LoginResultCode.AccountSuspended:
                return TextCatalog.LoginAccountSuspended;
            case LoginResultCode.AccountBanned:
                return TextCatalog.LoginAccountBanned;
            case LoginResultCode.ServiceUnavailable:
                return TextCatalog.CommonServerUnreachable;
            default:
                return TextCatalog.LoginCredentialsRejected;
        }
    }
}
