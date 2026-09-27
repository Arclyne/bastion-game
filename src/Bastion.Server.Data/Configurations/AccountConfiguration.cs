using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bastion.Server.Data.Conversions;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    private const int TypeLength = 10;
    private const int StatusLength = 10;
    private const int RoleLength = 13;
    private const int NicknameLength = 30;
    private const int EmailLength = 254;
    private const int PasswordHashLength = 64;
    private const int PasswordSaltLength = 32;
    private const int LanguageLength = 10;
    private const int TermsVersionLength = 20;

    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable(nameof(Account));
        builder.HasKey(account => account.AccountId);

        builder.Property(account => account.AccountType)
            .HasConversion(new UpperSnakeCaseEnumConverter<AccountType>())
            .HasMaxLength(TypeLength)
            .IsUnicode(false);
        builder.Property(account => account.AccountStatus)
            .HasConversion(new UpperSnakeCaseEnumConverter<AccountStatus>())
            .HasMaxLength(StatusLength)
            .IsUnicode(false);
        builder.Property(account => account.Role)
            .HasConversion(new UpperSnakeCaseEnumConverter<AccountRole>())
            .HasMaxLength(RoleLength)
            .IsUnicode(false);

        builder.Property(account => account.Nickname).HasMaxLength(NicknameLength);
        builder.Property(account => account.Email).HasMaxLength(EmailLength);
        builder.Property(account => account.PasswordHash).HasMaxLength(PasswordHashLength);
        builder.Property(account => account.PasswordSalt).HasMaxLength(PasswordSaltLength);
        builder.Property(account => account.PreferredLanguage).HasMaxLength(LanguageLength).IsUnicode(false);
        builder.Property(account => account.TermsVersion).HasMaxLength(TermsVersionLength).IsUnicode(false);
        builder.Property(account => account.TermsLanguage).HasMaxLength(LanguageLength).IsUnicode(false);

        builder.HasMany(account => account.ModeStatistics)
            .WithOne(statistic => statistic.Account)
            .HasForeignKey(statistic => statistic.AccountId);
    }
}
