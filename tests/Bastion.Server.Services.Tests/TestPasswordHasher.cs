using Bastion.Server.Services.Security;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestPasswordHasher
{
    private const string Password = "Luis#Quo2026";

    private readonly PasswordHasher _hasher = new PasswordHasher();

    [Fact]
    public void Matches_SamePassword_ReturnsTrue()
    {
        StoredPassword stored = _hasher.Hash(Password);

        bool isMatch = _hasher.Matches(Password, stored);

        Assert.True(isMatch);
    }

    [Fact]
    public void Matches_DifferentPassword_ReturnsFalse()
    {
        StoredPassword stored = _hasher.Hash(Password);

        bool isMatch = _hasher.Matches("Luis#Quo2027", stored);

        Assert.False(isMatch);
    }

    [Fact]
    public void Hash_SamePasswordTwice_UsesDifferentSalts()
    {
        StoredPassword first = _hasher.Hash(Password);

        StoredPassword second = _hasher.Hash(Password);

        Assert.NotEqual(first.Salt, second.Salt);
    }

    [Fact]
    public void Hash_AnyPassword_FitsTheHashColumn()
    {
        StoredPassword stored = _hasher.Hash(Password);

        int length = stored.Hash.Length;

        Assert.Equal(PasswordHasher.HashLength, length);
    }
}
