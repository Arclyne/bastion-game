using System;
using System.Data.Common;
using System.Threading.Tasks;
using CoreWCF;
using log4net;
using Microsoft.EntityFrameworkCore;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Auditing;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Network entry point for signing in (CU-01) and signing up (CU-02).
/// </summary>
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class AccountService : IAccountService
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(AccountService));

    private readonly ILoginService _loginService;
    private readonly IRegistrationService _registrationService;
    private readonly IAccessAuditor _auditor;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="loginService">Runs the sign-in flow.</param>
    /// <param name="registrationService">Runs the sign-up flow.</param>
    /// <param name="auditor">Records every attempt in the access log.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public AccountService(ILoginService loginService, IRegistrationService registrationService, IAccessAuditor auditor)
    {
        ArgumentNullException.ThrowIfNull(loginService);
        ArgumentNullException.ThrowIfNull(registrationService);
        ArgumentNullException.ThrowIfNull(auditor);

        _loginService = loginService;
        _registrationService = registrationService;
        _auditor = auditor;
    }

    /// <summary>
    /// Signs a player in with a nickname or an email and a password.
    /// </summary>
    /// <param name="request">The credentials and the device data.</param>
    /// <returns>The result code and, on success, the session token and the player profile.</returns>
    public async Task<LoginResult> LogInAsync(LoginRequest request)
    {
        if (request is null)
        {
            return new LoginResult { Code = LoginResultCode.InvalidCredentials };
        }

        try
        {
            LoginOutcome outcome = await _loginService.LogInAsync(request);
            AccessAttempt attempt = outcome.Attempt;
            await _auditor.RecordAsync(attempt);
            _logger.Info($"Sign-in attempt processed. Result={attempt.Result}; AccountId={attempt.AccountId}");
            return outcome.Result;
        }
        catch (DbUpdateException ex)
        {
            _logger.Error("Sign-in could not be saved.", ex);
            return new LoginResult { Code = LoginResultCode.ServiceUnavailable };
        }
        catch (DbException ex)
        {
            _logger.Error("Sign-in could not reach the database.", ex);
            return new LoginResult { Code = LoginResultCode.ServiceUnavailable };
        }
    }

    /// <summary>
    /// Registers a new player account.
    /// </summary>
    /// <param name="request">The registration data.</param>
    /// <returns>The result code of the registration.</returns>
    public async Task<RegistrationResult> RegisterAsync(RegistrationRequest request)
    {
        if (request is null)
        {
            return new RegistrationResult { Code = RegistrationResultCode.InvalidData };
        }

        try
        {
            RegistrationOutcome outcome = await _registrationService.RegisterAsync(request);
            await RecordRegistrationAsync(outcome, request.Nickname);
            _logger.Info($"Registration processed. Result={outcome.Code}; AccountId={outcome.AccountId}");
            return new RegistrationResult { Code = outcome.Code };
        }
        catch (DbUpdateException ex)
        {
            _logger.Error("Registration could not be saved.", ex);
            return new RegistrationResult { Code = RegistrationResultCode.ServiceUnavailable };
        }
        catch (DbException ex)
        {
            _logger.Error("Registration could not reach the database.", ex);
            return new RegistrationResult { Code = RegistrationResultCode.ServiceUnavailable };
        }
    }

    private Task RecordRegistrationAsync(RegistrationOutcome outcome, string nickname)
    {
        AccessLogResult result = outcome.Code == RegistrationResultCode.Registered
            ? AccessLogResult.RegistrationSucceeded
            : AccessLogResult.RegistrationRejected;
        return _auditor.RecordAsync(new AccessAttempt(result, outcome.AccountId, nickname));
    }
}
