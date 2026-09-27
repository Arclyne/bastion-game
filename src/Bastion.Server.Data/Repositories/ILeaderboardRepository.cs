using System.Collections.Generic;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public interface ILeaderboardRepository
{
    Task<IReadOnlyList<LeaderboardRow>> GetPageAsync(LeaderboardQuery query);

    Task<ModeStatistic?> FindStatisticAsync(int accountId, byte gameModeId);

    Task<int> CountAheadAsync(ModeStatistic statistic, int minimumMatches);
}
