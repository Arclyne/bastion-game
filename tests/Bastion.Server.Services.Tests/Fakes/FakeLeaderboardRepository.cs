using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;

namespace Bastion.Server.Services.Tests.Fakes;

public sealed class FakeLeaderboardRepository : ILeaderboardRepository
{
    public List<LeaderboardRow> Rows { get; } = [];

    public ModeStatistic? OwnStatistic { get; set; }

    public int PlayersAhead { get; set; }

    public Task<IReadOnlyList<LeaderboardRow>> GetPageAsync(LeaderboardQuery query)
    {
        IReadOnlyList<LeaderboardRow> page = Rows.Skip(query.Skip).Take(query.Take).ToList();
        return Task.FromResult(page);
    }

    public Task<ModeStatistic?> FindStatisticAsync(int accountId, byte gameModeId)
    {
        return Task.FromResult(OwnStatistic);
    }

    public Task<int> CountAheadAsync(ModeStatistic statistic, int minimumMatches)
    {
        return Task.FromResult(PlayersAhead);
    }
}
