using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Common;
using Bastion.Server.Services.Security;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Checks an identifier and a password against the stored account and applies the lockout (CU-01).
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private static readonly StoredPassword _decoyPassword = new StoredPassword(
        RandomNumberGenerator.GetBytes(PasswordHasher.HashLength),
        RandomNumberGenerator.GetBytes(PasswordHasher.SaltLength));

    private readonly IAccountRepository _accounts;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICallContext _callContext;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="accounts">Store of accounts.</param>
    /// <param name="passwordHasher">Checks password hashes.</param>
    /// <param name="callContext">Time of the current call.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public AuthenticationService(IAccountRepository accounts, IPasswordHasher passwordHasher, ICallContext callContext)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        ArgumentNullException.ThrowIfNull(passwordHasher);
        ArgumentNullException.ThrowIfNull(callContext);

        _accounts = accounts;
        _passwordHasher = passwordHasher;
        _callContext = callContext;
    }

    /// <summary>
    /// Authenticates a player by nickname or email.
    /// </summary>
    /// <param name="identifier">The nickname or the email typed by the player.</param>
    /// <param name="password">The password typed by the player.</param>
    /// <returns>The access result, the account when it was identified and the lock minutes left.</returns>
    /// <exception cref="ArgumentNullException">The identifier or the password is null.</exception>
    public async Task<AuthenticationOutcome> AuthenticateAsync(string identifier, string password)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        ArgumentNullException.ThrowIfNull(password);

        Account? account = await _accounts.FindByIdentifierAsync(identifier);
        if (account?.PasswordHash is null || account.PasswordSalt is null)
        {
            _passwordHasher.Matches(password, _decoyPassword);
            return new AuthenticationOutcome(AccessLogResult.AccountNotFound, null, 0);
        }

        DateTime now = _callContext.UtcNow;
        if (LockoutPolicy.IsLocked(account, now))
        {
            return new AuthenticationOutcome(
                AccessLogResult.AccountLocked,
                account,
                LockoutPolicy.GetRemainingMinutes(account, now));
        }

        var stored = new StoredPassword(account.PasswordHash, account.PasswordSalt);
        bool isMatch = _passwordHasher.Matches(password, stored);
        return isMatch ? await AcceptAsync(account) : await RejectAsync(account, now);
    }

    private async Task<AuthenticationOutcome> AcceptAsync(Account account)
    {
        if (account.FailedAttempts > 0 || account.LockedUntil is not null)
        {
            LockoutPolicy.RegisterSuccess(account);
            await _accounts.UpdateAsync(account);
        }

        return new AuthenticationOutcome(AccessLogResult.LoginSucceeded, account, 0);
    }

    private async Task<AuthenticationOutcome> RejectAsync(Account account, DateTime now)
    {
        LockoutPolicy.RegisterFailure(account, now);
        await _accounts.UpdateAsync(account);
        return new AuthenticationOutcome(
            AccessLogResult.WrongPassword,
            account,
            LockoutPolicy.GetRemainingMinutes(account, now));
    }
}
