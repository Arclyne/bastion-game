using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Bastion.Client.Localization;
using Bastion.Client.Networking;
using Bastion.Client.Session;
using Bastion.Contracts.Leaderboard;

namespace Bastion.Client.Screens.Leaderboard;

// Ranking logic of CU-35 without any MonoGame type: which modes can be picked, what to ask the server and how
// to show its answer in the current culture.
public sealed class LeaderboardController
{
    private const string NumberFormat = "N0";

    private static readonly string[] _gameModeCodes =
    [
        LeaderboardRules.ClassicModeCode,
        LeaderboardRules.FourPlayersModeCode,
        LeaderboardRules.QuickModeCode,
    ];

    private readonly ILeaderboardClient _leaderboard;
    private readonly SessionContext _session;

    public LeaderboardController(ILeaderboardClient leaderboard, SessionContext session)
    {
        ArgumentNullException.ThrowIfNull(leaderboard);
        ArgumentNullException.ThrowIfNull(session);

        _leaderboard = leaderboard;
        _session = session;
    }

    public static IReadOnlyList<string> GameModeCodes => _gameModeCodes;

    public string OwnNickname => _session.Nickname;

    public Task<LeaderboardPage> LoadAsync(string gameModeCode, int pageIndex)
    {
        ArgumentNullException.ThrowIfNull(gameModeCode);

        var request = new LeaderboardRequest
        {
            GameModeCode = gameModeCode,
            PageIndex = pageIndex,
            SessionToken = _session.SessionToken,
        };
        return _leaderboard.GetLeaderboardAsync(request);
    }

    public static IReadOnlyList<string> GetGameModeNames()
    {
        return _gameModeCodes.Select(GetGameModeName).ToList();
    }

    public static string GetGameModeName(string code)
    {
        switch (code)
        {
            case LeaderboardRules.FourPlayersModeCode:
                return TextCatalog.GameModeFourPlayers;
            case LeaderboardRules.QuickModeCode:
                return TextCatalog.GameModeQuick;
            default:
                return TextCatalog.GameModeClassic;
        }
    }

    public static LeaderboardRowView ToView(LeaderboardEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return new LeaderboardRowView(
            entry.Position.ToString(NumberFormat, CultureInfo.CurrentCulture),
            entry.Nickname,
            entry.EloRating.ToString(NumberFormat, CultureInfo.CurrentCulture),
            entry.MatchesPlayed.ToString(NumberFormat, CultureInfo.CurrentCulture),
            entry.MatchesWon.ToString(NumberFormat, CultureInfo.CurrentCulture));
    }

    // Null when the page has rows to show; otherwise the sentence that replaces the table.
    public static string? GetStatusText(LeaderboardPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        if (page.Code != LeaderboardResultCode.Success)
        {
            return TextCatalog.CommonServerUnreachable;
        }

        return page.Entries.Count == 0 ? TextCatalog.LeaderboardEmpty : null;
    }

    public static string GetNotRankedText(int matchesToQualify)
    {
        return string.Format(CultureInfo.CurrentCulture, TextCatalog.LeaderboardNotRankedFormat, matchesToQualify);
    }

    public static string GetPageText(int pageIndex)
    {
        return string.Format(CultureInfo.CurrentCulture, TextCatalog.LeaderboardPageFormat, pageIndex + 1);
    }
}
