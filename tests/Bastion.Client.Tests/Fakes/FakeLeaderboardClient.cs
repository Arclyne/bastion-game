using System.Threading.Tasks;
using Bastion.Client.Networking;
using Bastion.Contracts.Leaderboard;

namespace Bastion.Client.Tests.Fakes;

public sealed class FakeLeaderboardClient : ILeaderboardClient
{
    public LeaderboardRequest? LastRequest { get; private set; }

    public Task<LeaderboardPage> GetLeaderboardAsync(LeaderboardRequest request)
    {
        LastRequest = request;
        return Task.FromResult(new LeaderboardPage());
    }
}
