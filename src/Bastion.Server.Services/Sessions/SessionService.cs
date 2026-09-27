using System;
using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Common;
using Bastion.Server.Services.Security;

namespace Bastion.Server.Services.Sessions;

/// <summary>
/// Opens sessions after a successful sign-in and resolves session tokens (CU-01 step 12, RN-07).
/// </summary>
public sealed class SessionService : ISessionService
{
    private const int SessionLifetimeHours = 24;
    private const int MaximumFingerprintLength = 128;
    private const int MaximumClientVersionLength = 20;

    private readonly ISessionRepository _sessions;
    private readonly ISessionTokenFactory _tokenFactory;
    private readonly ICallContext _callContext;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="sessions">Store of sessions.</param>
    /// <param name="tokenFactory">Creates and hashes tokens.</param>
    /// <param name="callContext">Time and address of the current call.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public SessionService(ISessionRepository sessions, ISessionTokenFactory tokenFactory, ICallContext callContext)
    {
        ArgumentNullException.ThrowIfNull(sessions);
        ArgumentNullException.ThrowIfNull(tokenFactory);
        ArgumentNullException.ThrowIfNull(callContext);

        _sessions = sessions;
        _tokenFactory = tokenFactory;
        _callContext = callContext;
    }

    /// <summary>
    /// Opens a session that lasts twenty-four hours at most.
    /// </summary>
    /// <param name="accountId">The account that signed in.</param>
    /// <param name="request">The sign-in request, which carries the device data.</param>
    /// <returns>The token the client must send in later calls.</returns>
    /// <exception cref="ArgumentNullException">The request is null.</exception>
    public async Task<string> OpenAsync(int accountId, LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        IssuedToken issued = _tokenFactory.Create();
        DateTime now = _callContext.UtcNow;
        var session = new Session
        {
            AccountId = accountId,
            TokenHash = issued.TokenHash,
            StartedAt = now,
            ExpiresAt = now.AddHours(SessionLifetimeHours),
            LastUsedAt = now,
            IpAddress = _callContext.ClientAddress,
            DeviceFingerprint = Truncate(request.DeviceFingerprint, MaximumFingerprintLength),
            ClientVersion = Truncate(request.ClientVersion, MaximumClientVersionLength),
        };
        await _sessions.AddAsync(session);
        return issued.Token;
    }

    /// <summary>
    /// Finds the account behind an open, unexpired session.
    /// </summary>
    /// <param name="token">The token the client sent.</param>
    /// <returns>The account id, or <c>null</c> when the token is unknown, closed or expired.</returns>
    public async Task<int?> FindAccountIdAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        Session? session = await _sessions.FindOpenAsync(_tokenFactory.ComputeHash(token), _callContext.UtcNow);
        return session?.AccountId;
    }

    private static string Truncate(string? value, int maximumLength)
    {
        string text = value ?? string.Empty;
        return text.Length <= maximumLength ? text : text.Substring(0, maximumLength);
    }
}
