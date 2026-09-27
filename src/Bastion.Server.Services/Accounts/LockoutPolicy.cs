using System;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Services.Accounts;

// Escalating lockout of CU-01 RN-03: the 3rd consecutive failure locks for 5 minutes, the 6th for 30 and every one
// from the 9th on for 60. The counter restarts after a successful sign-in or 24 hours without failures.
public static class LockoutPolicy
{
    private const int FirstThreshold = 3;
    private const int SecondThreshold = 6;
    private const int ThirdThreshold = 9;
    private const int FirstLockMinutes = 5;
    private const int SecondLockMinutes = 30;
    private const int ThirdLockMinutes = 60;
    private const int CounterResetHours = 24;

    public static bool IsLocked(Account account, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(account);

        return account.LockedUntil is DateTime lockedUntil && lockedUntil > now;
    }

    public static int GetRemainingMinutes(Account account, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (account.LockedUntil is not DateTime lockedUntil || lockedUntil <= now)
        {
            return 0;
        }

        return (int)Math.Ceiling((lockedUntil - now).TotalMinutes);
    }

    public static void RegisterFailure(Account account, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(account);

        var resetAfter = TimeSpan.FromHours(CounterResetHours);
        if (account.LastFailedAttemptAt is DateTime lastFailure && now - lastFailure >= resetAfter)
        {
            account.FailedAttempts = 0;
        }

        if (account.FailedAttempts < byte.MaxValue)
        {
            account.FailedAttempts++;
        }

        account.LastFailedAttemptAt = now;
        int lockMinutes = GetLockMinutes(account.FailedAttempts);
        if (lockMinutes > 0)
        {
            account.LockedUntil = now.AddMinutes(lockMinutes);
        }
    }

    public static void RegisterSuccess(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        account.FailedAttempts = 0;
        account.LastFailedAttemptAt = null;
        account.LockedUntil = null;
    }

    private static int GetLockMinutes(int failedAttempts)
    {
        if (failedAttempts >= ThirdThreshold)
        {
            return ThirdLockMinutes;
        }

        if (failedAttempts == SecondThreshold)
        {
            return SecondLockMinutes;
        }

        return failedAttempts == FirstThreshold ? FirstLockMinutes : 0;
    }
}
