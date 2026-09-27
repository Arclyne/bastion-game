using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Common;
using Bastion.Server.Services.Security;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Builds a new registered account with its hashed password and one statistics row per active mode (CU-02).
/// </summary>
public sealed class AccountFactory : IAccountFactory
{
    // Version of the terms of use the registration form shows (CU-02 RN-12).
    private const string CurrentTermsVersion = "2026.2";
    private const short InitialEloRating = 1000;

    private readonly IPasswordHasher _passwordHasher;
    private readonly IGameModeRepository _gameModes;
    private readonly ICallContext _callContext;

    /// <summary>
    /// Creates the factory.
    /// </summary>
    /// <param name="passwordHasher">Hashes the password.</param>
    /// <param name="gameModes">Store of game modes.</param>
    /// <param name="callContext">Time of the current call.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public AccountFactory(IPasswordHasher passwordHasher, IGameModeRepository gameModes, ICallContext callContext)
    {
        ArgumentNullException.ThrowIfNull(passwordHasher);
        ArgumentNullException.ThrowIfNull(gameModes);
        ArgumentNullException.ThrowIfNull(callContext);

        _passwordHasher = passwordHasher;
        _gameModes = gameModes;
        _callContext = callContext;
    }

    /// <summary>
    /// Builds the account to save from validated registration data.
    /// </summary>
    /// <param name="request">Registration data that already passed validation.</param>
    /// <returns>The new account, not yet saved.</returns>
    /// <exception cref="ArgumentNullException">The request is null.</exception>
    public async Task<Account> CreateAsync(RegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        StoredPassword stored = _passwordHasher.Hash(request.Password);
        IReadOnlyList<GameMode> activeModes = await _gameModes.GetActiveAsync();
        DateTime now = _callContext.UtcNow;
        string language = LanguageCode.FromCultureName(request.PreferredLanguage);

        // Activated right away while there is no email service to verify the address (CU-04 comes later).
        return new Account
        {
            AccountType = AccountType.Registered,
            AccountStatus = AccountStatus.Active,
            Role = AccountRole.Player,
            Nickname = request.Nickname.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = stored.Hash,
            PasswordSalt = stored.Salt,
            BirthDate = DateOnly.FromDateTime(request.BirthDate),
            PreferredLanguage = language,
            RegisteredAt = now,
            TermsVersion = CurrentTermsVersion,
            TermsLanguage = language,
            TermsAcceptedAt = now,
            ModeStatistics = activeModes.Select(CreateStatistic).ToList(),
        };
    }

    private static ModeStatistic CreateStatistic(GameMode gameMode)
    {
        return new ModeStatistic { GameModeId = gameMode.GameModeId, EloRating = InitialEloRating };
    }
}
