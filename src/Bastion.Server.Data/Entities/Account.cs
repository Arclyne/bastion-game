using System;
using System.Collections.Generic;

namespace Bastion.Server.Data.Entities;

public class Account
{
    public int AccountId { get; set; }

    public AccountType AccountType { get; set; }

    public AccountStatus AccountStatus { get; set; }

    public AccountRole Role { get; set; }

    public string Nickname { get; set; } = string.Empty;

    public string? Email { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string PreferredLanguage { get; set; } = string.Empty;

    public byte FailedAttempts { get; set; }

    public DateTime? LastFailedAttemptAt { get; set; }

    public DateTime? LockedUntil { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? LastVerificationSentAt { get; set; }

    public DateTime? LastRecoverySentAt { get; set; }

    public string? TermsVersion { get; set; }

    public string? TermsLanguage { get; set; }

    public DateTime? TermsAcceptedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public ICollection<ModeStatistic> ModeStatistics { get; set; } = new List<ModeStatistic>();
}
