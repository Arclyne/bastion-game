using System;
using System.Threading.Tasks;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Networking;
using Bastion.Client.Validation;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Screens.Register;

public sealed class RegisterController
{
    private readonly IAccountClient _accounts;

    public RegisterController(IAccountClient accounts)
    {
        ArgumentNullException.ThrowIfNull(accounts);

        _accounts = accounts;
    }

    public static RegistrationWarnings Validate(RegistrationForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        return new RegistrationWarnings
        {
            Nickname = GetNicknameWarning(form.Nickname.Trim()),
            Email = InputRules.IsEmail(form.Email.Trim()) ? null : TextCatalog.RegisterEmailInvalid,
            Password = GetPasswordWarning(form.Password),
            Confirmation = form.Confirmation == form.Password ? null : TextCatalog.RegisterConfirmationMismatch,
            BirthDate = GetBirthDateWarning(form),
            Terms = form.HasAcceptedTerms ? null : TextCatalog.RegisterTermsRequired,
        };
    }

    public Task<RegistrationResult> RegisterAsync(RegistrationForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        InputRules.TryParseBirthDate(ReadBirthDate(form), out DateOnly birthDate);
        var request = new RegistrationRequest
        {
            Nickname = form.Nickname.Trim(),
            Email = form.Email.Trim(),
            Password = form.Password,
            BirthDate = birthDate.ToDateTime(TimeOnly.MinValue),
            PreferredLanguage = Language.Current.Name,
            HasAcceptedTerms = form.HasAcceptedTerms,
        };
        return _accounts.RegisterAsync(request);
    }

    public static RegistrationWarnings? GetFieldWarnings(RegistrationResultCode code)
    {
        switch (code)
        {
            case RegistrationResultCode.NicknameTaken:
                return new RegistrationWarnings { Nickname = TextCatalog.RegisterNicknameTaken };
            case RegistrationResultCode.EmailTaken:
                return new RegistrationWarnings { Email = TextCatalog.RegisterEmailTaken };
            case RegistrationResultCode.Underage:
                return new RegistrationWarnings { BirthDate = TextCatalog.RegisterUnderage };
            case RegistrationResultCode.TermsNotAccepted:
                return new RegistrationWarnings { Terms = TextCatalog.RegisterTermsRequired };
            default:
                return null;
        }
    }

    public static string GetFailureMessage(RegistrationResultCode code)
    {
        return code == RegistrationResultCode.ServiceUnavailable
            ? TextCatalog.CommonServerUnreachable
            : TextCatalog.RegisterRejected;
    }

    private static string? GetNicknameWarning(string nickname)
    {
        if (!InputRules.HasNicknameLength(nickname))
        {
            return TextCatalog.RegisterNicknameLength;
        }

        return InputRules.IsNickname(nickname) ? null : TextCatalog.RegisterNicknameInvalid;
    }

    private static string? GetPasswordWarning(string password)
    {
        if (!InputRules.HasPasswordLength(password))
        {
            return TextCatalog.RegisterPasswordTooShort;
        }

        return InputRules.MeetsPasswordPolicy(password) ? null : TextCatalog.RegisterPasswordPolicy;
    }

    private static string? GetBirthDateWarning(RegistrationForm form)
    {
        if (!InputRules.TryParseBirthDate(ReadBirthDate(form), out DateOnly date))
        {
            return TextCatalog.RegisterBirthDateInvalid;
        }

        if (InputRules.IsInFuture(date))
        {
            return TextCatalog.RegisterBirthDateFuture;
        }

        return InputRules.IsOldEnough(date) ? null : TextCatalog.RegisterUnderage;
    }

    private static BirthDateFields ReadBirthDate(RegistrationForm form)
    {
        return new BirthDateFields { Day = form.Day, Month = form.Month, Year = form.Year };
    }
}
