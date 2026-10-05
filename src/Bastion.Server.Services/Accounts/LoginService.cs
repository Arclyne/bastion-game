using System;
using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Auditing;
using Bastion.Server.Services.Sessions;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Runs the sign-in flow of CU-01: authentication, account status and session.
/// </summary>
public sealed class LoginService : ILoginService
{
    private readonly IAuthenticationService _authentication;
    private readonly ISessionService _sessions;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="authentication">Checks the credentials.</param>
    /// <param name="sessions">Opens the session.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public LoginService(IAuthenticationService authentication, ISessionService sessions)
    {
        ArgumentNullException.ThrowIfNull(authentication);
        ArgumentNullException.ThrowIfNull(sessions);

        _authentication = authentication;
        _sessions = sessions;
    }

    /// <summary>
    /// Signs a player in.
    /// </summary>
    /// <param name="request">The identifier, the password and the device data.</param>
    /// <returns>The result for the client and the attempt to record in the access log.</returns>
    /// <exception cref="ArgumentNullException">The request is null.</exception>
    public async Task<LoginOutcome> LogInAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string identifier = (request.Identifier ?? string.Empty).Trim();
        string password = request.Password ?? string.Empty;
        if (identifier.Length == 0 || password.Length == 0)
        {
            var emptyAttempt = new AccessAttempt(AccessLogResult.AccountNotFound, null, identifier);
            return Refuse(LoginResultCode.InvalidCredentials, emptyAttempt);
        }

        AuthenticationOutcome outcome = await _authentication.AuthenticateAsync(identifier, password);
        var attempt = new AccessAttempt(outcome.Result, outcome.Account?.AccountId, identifier);
        if (outcome.Result != AccessLogResult.LoginSucceeded || outcome.Account is null)
        {
            return RefuseCredentials(outcome, attempt);
        }

        LoginResultCode statusCode = MapStatus(outcome.Account.AccountStatus);
        if (statusCode != LoginResultCode.Success)
        {
            var refused = new LoginResult
            {
                Code = statusCode,
                Email = statusCode == LoginResultCode.AccountPending ? outcome.Account.Email : null,
            };
            return new LoginOutcome(refused, attempt with { Result = MapStatusResult(statusCode) });
        }

        string token = await _sessions.OpenAsync(outcome.Account.AccountId, request);
        var result = new LoginResult
        {
            Code = LoginResultCode.Success,
            SessionToken = token,
            Nickname = outcome.Account.Nickname,
            PreferredLanguage = outcome.Account.PreferredLanguage,
        };
        return new LoginOutcome(result, attempt);
    }

    private static LoginOutcome RefuseCredentials(AuthenticationOutcome outcome, AccessAttempt attempt)
    {
        LoginResultCode code = outcome.LockMinutes > 0
            ? LoginResultCode.AccountLocked
            : LoginResultCode.InvalidCredentials;
        var result = new LoginResult { Code = code, LockMinutes = outcome.LockMinutes };
        return new LoginOutcome(result, attempt);
    }

    private static LoginOutcome Refuse(LoginResultCode code, AccessAttempt attempt)
    {
        return new LoginOutcome(new LoginResult { Code = code }, attempt);
    }

    // Only ACTIVE accounts may sign in; each other status has its own message (CU-01 RN-04).
    private static LoginResultCode MapStatus(AccountStatus status)
    {
        switch (status)
        {
            case AccountStatus.Active:
                return LoginResultCode.Success;
            case AccountStatus.Pending:
                return LoginResultCode.AccountPending;
            case AccountStatus.Suspended:
                return LoginResultCode.AccountSuspended;
            case AccountStatus.Banned:
                return LoginResultCode.AccountBanned;
            default:
                return LoginResultCode.InvalidCredentials;
        }
    }

    private static AccessLogResult MapStatusResult(LoginResultCode code)
    {
        switch (code)
        {
            case LoginResultCode.AccountPending:
                return AccessLogResult.AccountPending;
            case LoginResultCode.AccountSuspended:
            case LoginResultCode.AccountBanned:
                return AccessLogResult.AccountSanctioned;
            default:
                return AccessLogResult.AccountNotFound;
        }
    }
}
