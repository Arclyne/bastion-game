using Bastion.Client.Networking;
using Bastion.Client.Session;

namespace Bastion.Client;

public sealed class ClientServices
{
    public required IAccountClient Accounts { get; init; }

    public required ILeaderboardClient Leaderboard { get; init; }

    public required SessionContext Session { get; init; }
}
