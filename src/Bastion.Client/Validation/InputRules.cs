using System;
using System.Linq;
using System.Text.RegularExpressions;
using Bastion.Client.Controls;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Validation;

// Checks the forms run before sending anything, so obvious mistakes show at once instead of after a round trip
// (CU-02 FA-03, FA-04). The server repeats them: nothing here is trusted on its own. Rules shared with the server
// live in AccountRules.
public static partial class InputRules
{
    public const int EmptyStrength = 0;
    public const int WeakStrength = 1;
    public const int FairStrength = 2;
    public const int StrongStrength = 3;

    private const int EarliestBirthYear = 1900;
    private const int FirstMonth = 1;
    private const int LastMonth = 12;
    private const int FirstDay = 1;

    private static readonly string[] _shorteners =
        ["bit.ly", "tinyurl.com", "t.co", "goo.gl", "cutt.ly", "rb.gy", "is.gd", "ow.ly"];

    // Unicode letters, because players are called Nuñez and O'Brien; marks cover accents that compose.
    [GeneratedRegex(@"^\p{L}[\p{L}\p{M}'\- ]{0,49}$")]
    private static partial Regex PersonNamePattern();

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsPattern();

    public static bool IsNickname(string nickname)
    {
        ArgumentNullException.ThrowIfNull(nickname);

        return AccountRules.IsNickname(nickname);
    }

    public static bool IsPersonName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return PersonNamePattern().IsMatch(name.Trim());
    }

    public static bool IsWholeNumber(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return DigitsPattern().IsMatch(text);
    }

    public static bool HasNicknameLength(string nickname)
    {
        ArgumentNullException.ThrowIfNull(nickname);

        return nickname.Length >= AccountRules.MinimumNicknameLength
            && nickname.Length <= AccountRules.MaximumNicknameLength;
    }

    public static bool HasPasswordLength(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return AccountRules.HasPasswordLength(password);
    }

    public static bool IsEmail(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return AccountRules.IsEmail(email);
    }

    public static bool TryParseBirthDate(BirthDateFields fields, out DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(fields);

        date = default;

        // Digits only: int.TryParse would accept " 12" and "+12", which the three date boxes never mean.
        bool hasDigitsOnly = IsWholeNumber(fields.Day) && IsWholeNumber(fields.Month) && IsWholeNumber(fields.Year);
        if (!hasDigitsOnly
            || !int.TryParse(fields.Day, out int day)
            || !int.TryParse(fields.Month, out int month)
            || !int.TryParse(fields.Year, out int year))
        {
            return false;
        }

        if (!IsValidDate(year, month, day))
        {
            return false;
        }

        date = new DateOnly(year, month, day);
        return true;
    }

    public static int CountPasswordGroups(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return AccountRules.CountPasswordGroups(password);
    }

    public static bool MeetsPasswordPolicy(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return AccountRules.MeetsPasswordPolicy(password);
    }

    public static int GetPasswordStrength(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        if (password.Length == 0)
        {
            return EmptyStrength;
        }

        if (!HasPasswordLength(password))
        {
            return WeakStrength;
        }

        return MeetsPasswordPolicy(password) ? StrongStrength : FairStrength;
    }

    public static bool IsWebAddress(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? address)
            && address.Scheme == Uri.UriSchemeHttps
            && address.Host.Contains('.', StringComparison.Ordinal);
    }

    public static bool IsShortener(string url)
    {
        ArgumentNullException.ThrowIfNull(url);

        return Uri.TryCreate(url, UriKind.Absolute, out Uri? address)
            && _shorteners.Contains(address.Host, StringComparer.OrdinalIgnoreCase);
    }

    public static bool IsInFuture(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.Today);
    }

    public static bool IsOldEnough(DateOnly birthDate)
    {
        return AccountRules.IsOldEnough(birthDate.ToDateTime(TimeOnly.MinValue), DateTime.Today);
    }

    private static bool IsValidDate(int year, int month, int day)
    {
        return year >= EarliestBirthYear
            && month >= FirstMonth
            && month <= LastMonth
            && day >= FirstDay
            && day <= DateTime.DaysInMonth(year, month);
    }
}
