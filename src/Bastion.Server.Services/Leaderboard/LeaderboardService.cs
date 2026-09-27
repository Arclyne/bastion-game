using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF;
using log4net;
using Bastion.Contracts.Leaderboard;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Sessions;

namespace Bastion.Server.Services.Leaderboard;

/// <summary>
/// Network entry point for the ranking of a game mode (CU-35).
/// </summary>
[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class LeaderboardService : ILeaderboardService
{
    private const int MinimumMatches = LeaderboardRules.MinimumMatches;
    private const int PageSize = LeaderboardRules.PageSize;

    private static readonly ILog _logger = LogManager.GetLogger(typeof(LeaderboardService));

    private readonly IGameModeRepository _gameModes;
    private readonly ILeaderboardRepository _leaderboard;
    private readonly ISessionService _sessions;

    /// <summary>
    /// Creates the service.
    /// </summary>
    /// <param name="gameModes">Store of game modes.</param>
    /// <param name="leaderboard">Ranking queries.</param>
    /// <param name="sessions">Resolves the session token to find the player's own row.</param>
    /// <exception cref="ArgumentNullException">A dependency is null.</exception>
    public LeaderboardService(
        IGameModeRepository gameModes,
        ILeaderboardRepository leaderboard,
        ISessionService sessions)
    {
        ArgumentNullException.ThrowIfNull(gameModes);
        ArgumentNullException.ThrowIfNull(leaderboard);
        ArgumentNullException.ThrowIfNull(sessions);

        _gameModes = gameModes;
        _leaderboard = leaderboard;
        _sessions = sessions;
    }

    /// <summary>
    /// Gets one page of the ranking of a game mode and, for a signed-in player, their own position.
    /// </summary>
    /// <param name="request">The mode code, the page and the optional session token.</param>
    /// <returns>The page of ranked players and the player's own row.</returns>
    public async Task<LeaderboardPage> GetLeaderboardAsync(LeaderboardRequest request)
    {
        if (request is null)
        {
            return new LeaderboardPage { Code = LeaderboardResultCode.UnknownGameMode };
        }

        try
        {
            GameMode? gameMode = await _gameModes.FindByCodeAsync(request.GameModeCode ?? string.Empty);
            if (gameMode is null)
            {
                return new LeaderboardPage { Code = LeaderboardResultCode.UnknownGameMode };
            }

            LeaderboardPage page = await GetPageAsync(gameMode.GameModeId, Math.Max(0, request.PageIndex));
            await AddOwnEntryAsync(page, gameMode.GameModeId, request.SessionToken);
            return page;
        }
        catch (DbException ex)
        {
            _logger.Error($"Ranking could not reach the database. GameModeCode={request.GameModeCode}", ex);
            return new LeaderboardPage { Code = LeaderboardResultCode.ServiceUnavailable };
        }
    }

    private async Task<LeaderboardPage> GetPageAsync(byte gameModeId, int pageIndex)
    {
        int skip = pageIndex * PageSize;
        var query = new LeaderboardQuery
        {
            GameModeId = gameModeId,
            MinimumMatches = MinimumMatches,
            Skip = skip,
            Take = PageSize + 1,
        };
        IReadOnlyList<LeaderboardRow> rows = await _leaderboard.GetPageAsync(query);
        var entries = rows
            .Take(PageSize)
            .Select((row, index) => CreateEntry(row, skip + index + 1))
            .ToList();
        return new LeaderboardPage
        {
            Code = LeaderboardResultCode.Success,
            Entries = entries,
            HasNextPage = rows.Count > PageSize,
        };
    }

    private async Task AddOwnEntryAsync(LeaderboardPage page, byte gameModeId, string? sessionToken)
    {
        int? accountId = await _sessions.FindAccountIdAsync(sessionToken ?? string.Empty);
        if (accountId is not int ownAccountId)
        {
            return;
        }

        ModeStatistic? statistic = await _leaderboard.FindStatisticAsync(ownAccountId, gameModeId);
        if (statistic?.Account is null)
        {
            return;
        }

        if (statistic.MatchesPlayed < MinimumMatches)
        {
            page.OwnMatchesToQualify = MinimumMatches - statistic.MatchesPlayed;
            return;
        }

        int position = await _leaderboard.CountAheadAsync(statistic, MinimumMatches) + 1;
        var row = new LeaderboardRow(
            statistic.Account.Nickname,
            statistic.EloRating,
            statistic.MatchesPlayed,
            statistic.MatchesWon);
        page.OwnEntry = CreateEntry(row, position);
    }

    private static LeaderboardEntry CreateEntry(LeaderboardRow row, int position)
    {
        return new LeaderboardEntry
        {
            Position = position,
            Nickname = row.Nickname,
            EloRating = row.EloRating,
            MatchesPlayed = row.MatchesPlayed,
            MatchesWon = row.MatchesWon,
        };
    }
}
