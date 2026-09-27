using System;
using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Common;

namespace Bastion.Server.Services.Accounts;

/// <summary>
/// Repeats on the server every check the registration form runs, and checks that the nickname and the email are
/// free (CU-02 RN-01, RN-02, RN-03, RN-05, RN-12).
/// </summary>
public sealed class RegistrationValidator : IRegistrationValidator
{
    private readonly IAccountRepository _accounts;
    private readonly ICallContext _callContext;

    /// <summary>
    /// Creates the validator.
    /// </summary>
    /// <param name="accounts">Store of accounts, to check that the nickname and the email are free.</param>
    /// <param name="callContext">Date of the current call, for the minimum age.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public RegistrationValidator(IAccountRepository accounts, ICallContext callContext)
    {
        ArgumentNullException.ThrowIfNull(accounts);
        ArgumentNullException.ThrowIfNull(callContext);

        _accounts = accounts;
        _callContext = callContext;
    }

    /// <summary>
    /// Finds the first problem that prevents the registration.
    /// </summary>
    /// <param name="request">The registration data sent by the client.</param>
    /// <returns>The code of the first problem found, or <c>null</c> when the data can be saved.</returns>
    /// <exception cref="ArgumentNullException">The request is null.</exception>
    public async Task<RegistrationResultCode?> FindProblemAsync(RegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegistrationResultCode? shapeProblem = FindShapeProblem(request, _callContext.UtcNow);
        if (shapeProblem is not null)
        {
            return shapeProblem;
        }

        if (await _accounts.IsNicknameTakenAsync(request.Nickname.Trim()))
        {
            return RegistrationResultCode.NicknameTaken;
        }

        bool isEmailTaken = await _accounts.IsEmailTakenAsync(request.Email.Trim().ToLowerInvariant());
        return isEmailTaken ? RegistrationResultCode.EmailTaken : null;
    }

    private static RegistrationResultCode? FindShapeProblem(RegistrationRequest request, DateTime today)
    {
        bool hasValidData = AccountRules.IsNickname((request.Nickname ?? string.Empty).Trim())
            && AccountRules.IsEmail((request.Email ?? string.Empty).Trim())
            && AccountRules.MeetsPasswordPolicy(request.Password ?? string.Empty)
            && request.BirthDate.Date <= today.Date;
        if (!hasValidData)
        {
            return RegistrationResultCode.InvalidData;
        }

        if (!AccountRules.IsOldEnough(request.BirthDate, today))
        {
            return RegistrationResultCode.Underage;
        }

        return request.HasAcceptedTerms ? null : RegistrationResultCode.TermsNotAccepted;
    }
}
