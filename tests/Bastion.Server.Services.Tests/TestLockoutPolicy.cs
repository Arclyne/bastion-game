using System;
using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Accounts;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestLockoutPolicy
{
    private static readonly DateTime _now = new DateTime(2026, 9, 27, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void RegisterFailure_ThirdFailure_LocksForFiveMinutes()
    {
        var account = new Account { FailedAttempts = 2, LastFailedAttemptAt = _now.AddMinutes(-1) };

        LockoutPolicy.RegisterFailure(account, _now);

        Assert.Equal(_now.AddMinutes(5), account.LockedUntil);
    }

    [Fact]
    public void RegisterFailure_SixthFailure_LocksForThirtyMinutes()
    {
        var account = new Account { FailedAttempts = 5, LastFailedAttemptAt = _now.AddMinutes(-1) };

        LockoutPolicy.RegisterFailure(account, _now);

        Assert.Equal(_now.AddMinutes(30), account.LockedUntil);
    }

    [Fact]
    public void RegisterFailure_FourthFailure_DoesNotLock()
    {
        var account = new Account { FailedAttempts = 3, LastFailedAttemptAt = _now.AddMinutes(-10) };

        LockoutPolicy.RegisterFailure(account, _now);

        Assert.Null(account.LockedUntil);
    }

    [Fact]
    public void RegisterFailure_AfterTwentyFourHours_RestartsCounter()
    {
        var account = new Account { FailedAttempts = 4, LastFailedAttemptAt = _now.AddHours(-25) };

        LockoutPolicy.RegisterFailure(account, _now);

        Assert.Equal(1, account.FailedAttempts);
    }

    [Fact]
    public void RegisterSuccess_LockedAccount_ClearsTheLock()
    {
        var account = new Account { FailedAttempts = 3, LockedUntil = _now.AddMinutes(5) };

        LockoutPolicy.RegisterSuccess(account);

        Assert.False(LockoutPolicy.IsLocked(account, _now));
    }

    [Fact]
    public void GetRemainingMinutes_LockedForNinetySeconds_RoundsUp()
    {
        var account = new Account { LockedUntil = _now.AddSeconds(90) };

        int minutes = LockoutPolicy.GetRemainingMinutes(account, _now);

        Assert.Equal(2, minutes);
    }
}
