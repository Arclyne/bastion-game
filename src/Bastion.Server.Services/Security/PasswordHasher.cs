using System;
using System.Security.Cryptography;
using System.Text;

namespace Bastion.Server.Services.Security;

/// <summary>
/// Derives and checks password hashes with PBKDF2 over SHA-512 (CU-01 RN-02).
/// </summary>
/// <remarks>
/// The sizes are fixed by the schema: Account.PasswordHash is VARBINARY(64) and Account.PasswordSalt is
/// VARBINARY(32).
/// </remarks>
public sealed class PasswordHasher : IPasswordHasher
{
    public const int HashLength = 64;
    public const int SaltLength = 32;

    private const int Iterations = 210_000;

    /// <summary>
    /// Hashes a password with a new random salt.
    /// </summary>
    /// <param name="password">The password in plain text; it is never stored.</param>
    /// <returns>The hash and the salt to store.</returns>
    /// <exception cref="ArgumentException">The password is null or empty.</exception>
    public StoredPassword Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        byte[] salt = RandomNumberGenerator.GetBytes(SaltLength);
        return new StoredPassword(Derive(password, salt), salt);
    }

    /// <summary>
    /// Checks a password against a stored hash in constant time.
    /// </summary>
    /// <param name="password">The password typed by the player.</param>
    /// <param name="stored">The stored hash and salt.</param>
    /// <returns><c>true</c> when the password produces the stored hash.</returns>
    /// <exception cref="ArgumentNullException">The stored password is null.</exception>
    public bool Matches(string password, StoredPassword stored)
    {
        ArgumentNullException.ThrowIfNull(stored);

        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        // A length or prefix shortcut would leak how much of the hash was right.
        return CryptographicOperations.FixedTimeEquals(Derive(password, stored.Salt), stored.Hash);
    }

    private static byte[] Derive(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            HashLength);
    }
}
