using Bastion.Client.Networking;
using Bastion.Client.Rendering;
using Bastion.Client.Session;

namespace Bastion.Client;

public sealed class ClientServices
{
    public required IAccountClient Accounts { get; init; }

    public required ILeaderboardClient Leaderboard { get; init; }

    public required SessionContext Session { get; init; }

    // Attached once the graphics device exists, which is later than the rest.
    public BoardRenderer? Board { get; private set; }

    public void AttachBoard(BoardRenderer? board)
    {
        Board = board;
    }
}
