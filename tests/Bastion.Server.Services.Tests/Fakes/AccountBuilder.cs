using Bastion.Server.Data.Entities;
using Bastion.Server.Services.Security;

namespace Bastion.Server.Services.Tests.Fakes;

public static class AccountBuilder
{
    public const string Nickname = "luis_quo";
    public const string Email = "luis.quo@example.test";
    public const string Password = "Luis#Quo2026";

    private static readonly StoredPassword _storedPassword = new PasswordHasher().Hash(Password);

    public static Account Create(AccountStatus status)
    {
        return new Account
        {
            AccountId = 3,
            AccountStatus = status,
            Nickname = Nickname,
            Email = Email,
            PasswordHash = _storedPassword.Hash,
            PasswordSalt = _storedPassword.Salt,
            PreferredLanguage = "es-MX",
        };
    }
}
