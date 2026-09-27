using System;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Session;

// What the client keeps after signing in; the server stays the source of truth for everything else.
public sealed class SessionContext
{
    public string? SessionToken { get; private set; }

    public string Nickname { get; private set; } = string.Empty;

    public bool IsSignedIn => SessionToken is not null;

    public void Start(LoginResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        SessionToken = result.SessionToken;
        Nickname = result.Nickname ?? string.Empty;
    }

    public void End()
    {
        SessionToken = null;
        Nickname = string.Empty;
    }
}
