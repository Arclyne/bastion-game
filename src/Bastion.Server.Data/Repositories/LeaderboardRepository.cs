using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

// Ranking order follows CU-35: highest elo first; on a tie, whoever reached that elo earlier (RN-06).
// The Account navigation is never null in these queries: FK_ModeStatistic_Account requires the account row.
public sealed class LeaderboardRepository : ILeaderboardRepository
{
    private readonly IDbContextFactory<BastionDbContext> _contextFactory;

    public LeaderboardRepository(IDbContextFactory<BastionDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<LeaderboardRow>> GetPageAsync(LeaderboardQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await QueryRanked(context, query.GameModeId, query.MinimumMatches)
            .OrderByDescending(statistic => statistic.EloRating)
            .ThenBy(statistic => statistic.LastMatchAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(statistic => new LeaderboardRow(
                statistic.Account!.Nickname,
                statistic.EloRating,
                statistic.MatchesPlayed,
                statistic.MatchesWon))
            .ToListAsync();
    }

    public async Task<ModeStatistic?> FindStatisticAsync(int accountId, byte gameModeId)
    {
        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.ModeStatistics
            .AsNoTracking()
            .Include(statistic => statistic.Account)
            .FirstOrDefaultAsync(statistic => statistic.AccountId == accountId && statistic.GameModeId == gameModeId);
    }

    public async Task<int> CountAheadAsync(ModeStatistic statistic, int minimumMatches)
    {
        ArgumentNullException.ThrowIfNull(statistic);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await QueryRanked(context, statistic.GameModeId, minimumMatches)
            .CountAsync(other => other.EloRating > statistic.EloRating
                || (other.EloRating == statistic.EloRating && other.LastMatchAt < statistic.LastMatchAt));
    }

    private static IQueryable<ModeStatistic> QueryRanked(BastionDbContext context, byte gameModeId, int minimumMatches)
    {
        return context.ModeStatistics
            .AsNoTracking()
            .Where(statistic => statistic.GameModeId == gameModeId
                && statistic.MatchesPlayed >= minimumMatches
                && statistic.Account!.AccountStatus == AccountStatus.Active);
    }
}
