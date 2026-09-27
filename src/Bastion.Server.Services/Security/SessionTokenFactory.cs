using System;
using System.Security.Cryptography;
using System.Text;

namespace Bastion.Server.Services.Security;

/// <summary>
/// Creates random session tokens; only their SHA-256 hash is stored in Session.TokenHash.
/// </summary>
public sealed class SessionTokenFactory : ISessionTokenFactory
{
    private const int TokenLength = 32;

    /// <summary>
    /// Creates a new random token together with the hash to store.
    /// </summary>
    /// <returns>The token for the client and its hash for the database.</returns>
    public IssuedToken Create()
    {
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenLength));
        return new IssuedToken(token, ComputeHash(token));
    }

    /// <summary>
    /// Computes the hash under which a token is stored.
    /// </summary>
    /// <param name="token">The token sent by the client.</param>
    /// <returns>The SHA-256 hash of the token.</returns>
    /// <exception cref="ArgumentNullException">The token is null.</exception>
    public byte[] ComputeHash(string token)
    {
        ArgumentNullException.ThrowIfNull(token);

        return SHA256.HashData(Encoding.UTF8.GetBytes(token));
    }
}
