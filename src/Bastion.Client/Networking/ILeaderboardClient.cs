using System.Threading.Tasks;
using Bastion.Contracts.Leaderboard;

namespace Bastion.Client.Networking;

public interface ILeaderboardClient
{
    Task<LeaderboardPage> GetLeaderboardAsync(LeaderboardRequest request);
}
