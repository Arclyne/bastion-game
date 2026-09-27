using System.ServiceModel;
using System.Threading.Tasks;

namespace Bastion.Contracts.Leaderboard;

[ServiceContract]
public interface ILeaderboardService
{
    [OperationContract]
    Task<LeaderboardPage> GetLeaderboardAsync(LeaderboardRequest request);
}
