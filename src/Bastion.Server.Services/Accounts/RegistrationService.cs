using System;
using System.Threading.Tasks;
using log4net;
using Microsoft.EntityFrameworkCore;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Runs the sign-up flow of CU-02: validation, uniqueness and saving the account.
/// </summary>
public sealed class RegistrationService : IRegistrationService
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(RegistrationService));

    private readonly IRegistrationValidator _validator;
    private readonly IAccountFactory _accountFactory;
    private readonly IAccountRepository _accounts;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="validator">Checks the registration data.</param>
    /// <param name="accountFactory">Builds the account to save.</param>
    /// <param name="accounts">Store of accounts.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public RegistrationService(
        IRegistrationValidator validator,
        IAccountFactory accountFactory,
        IAccountRepository accounts)
    {
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentNullException.ThrowIfNull(accountFactory);
        ArgumentNullException.ThrowIfNull(accounts);

        _validator = validator;
        _accountFactory = accountFactory;
        _accounts = accounts;
    }

    /// <summary>
    /// Registers a new player account.
    /// </summary>
    /// <param name="request">The registration data sent by the client.</param>
    /// <returns>The result code and, when the account was saved, its id.</returns>
    /// <exception cref="ArgumentNullException">The request is null.</exception>
    public async Task<RegistrationOutcome> RegisterAsync(RegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegistrationResultCode? problem = await _validator.FindProblemAsync(request);
        if (problem is RegistrationResultCode code)
        {
            return new RegistrationOutcome(code, null);
        }

        Account account = await _accountFactory.CreateAsync(request);
        try
        {
            await _accounts.AddAsync(account);
        }
        catch (DbUpdateException ex)
        {
            _logger.Warn($"Registration hit a unique index. Nickname={account.Nickname}", ex);
            RegistrationResultCode conflict = await FindConflictAsync(account);
            return new RegistrationOutcome(conflict, null);
        }

        return new RegistrationOutcome(RegistrationResultCode.Registered, account.AccountId);
    }

    private async Task<RegistrationResultCode> FindConflictAsync(Account account)
    {
        if (await _accounts.IsNicknameTakenAsync(account.Nickname))
        {
            return RegistrationResultCode.NicknameTaken;
        }

        return RegistrationResultCode.EmailTaken;
    }
}
