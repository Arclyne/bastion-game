using System;
using System.Globalization;
using System.Threading.Tasks;
using Bastion.Client.Localization;
using Bastion.Client.Networking;
using Bastion.Client.Screens;
using Bastion.Client.Session;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Screens.Login;

// Sign-in logic of CU-01 without any MonoGame type, so it can be tested on its own.
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

    // Local check only: it never reaches the server, so it does not count as a failed attempt (CU-01 RN-13).
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

    // Where each answer leads (CU-01): the hub, the pending-verification notice or the sanction screen. Null means
    // the player stays on the form and reads a message.
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

    // A wrong identifier and a wrong password share one message on purpose (CU-01 RN-01).
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
