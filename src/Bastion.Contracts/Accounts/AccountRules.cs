using System;
using System.Text.RegularExpressions;

namespace Bastion.Contracts.Accounts;

// Shape rules shared by the client, which checks them before sending, and the server, which checks them again
// because nothing coming from a client is trusted (CU-02 RN-01, RN-02, RN-03, RN-05).
public static class AccountRules
{
    public const int MinimumNicknameLength = 3;
    public const int MaximumNicknameLength = 30;
    public const int MaximumEmailLength = 254;
    public const int MinimumPasswordLength = 8;
    public const int RequiredPasswordGroups = 3;
    public const int MinimumAge = 8;

    private static readonly TimeSpan _matchTimeout = TimeSpan.FromMilliseconds(250);

    private static readonly Regex _nicknamePattern = new Regex(
        @"^[A-Za-z0-9_]{3,30}$",
        RegexOptions.CultureInvariant,
        _matchTimeout);

    // Stricter than the address grammar on purpose: one at sign, a host with a dot and no spaces.
    private static readonly Regex _emailPattern = new Regex(
        @"^[^@\s]+@[^@\s.]+(?:\.[^@\s.]+)+$",
        RegexOptions.CultureInvariant,
        _matchTimeout);

    private static readonly Regex[] _passwordGroupPatterns =
    {
        new Regex("[a-z]", RegexOptions.CultureInvariant, _matchTimeout),
        new Regex("[A-Z]", RegexOptions.CultureInvariant, _matchTimeout),
        new Regex(@"\d", RegexOptions.CultureInvariant, _matchTimeout),
        new Regex("[^A-Za-z0-9]", RegexOptions.CultureInvariant, _matchTimeout),
    };

    public static bool IsNickname(string nickname)
    {
        return nickname is not null && _nicknamePattern.IsMatch(nickname);
    }

    public static bool IsEmail(string email)
    {
        return email is not null && email.Length <= MaximumEmailLength && _emailPattern.IsMatch(email);
    }

    public static bool HasPasswordLength(string password)
    {
        return password is not null && password.Length >= MinimumPasswordLength;
    }

    public static int CountPasswordGroups(string password)
    {
        if (password is null)
        {
            return 0;
        }

        int groupCount = 0;
        foreach (Regex pattern in _passwordGroupPatterns)
        {
            if (pattern.IsMatch(password))
            {
                groupCount++;
            }
        }

        return groupCount;
    }

    public static bool MeetsPasswordPolicy(string password)
    {
        return HasPasswordLength(password) && CountPasswordGroups(password) >= RequiredPasswordGroups;
    }

    // Whole years only: a birth date is never off by a fraction of a year for the minimum age (CU-02 RN-05).
    public static bool IsOldEnough(DateTime birthDate, DateTime today)
    {
        return birthDate.Date.AddYears(MinimumAge) <= today.Date;
    }
}
