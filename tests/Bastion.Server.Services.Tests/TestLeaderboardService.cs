using System.Linq;
using System.Threading.Tasks;
using Bastion.Contracts.Leaderboard;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Leaderboard;
using Bastion.Server.Services.Tests.Fakes;
using Xunit;

namespace Bastion.Server.Services.Tests;

public sealed class TestLeaderboardService
{
    private const int OwnAccountId = 3;

    private readonly FakeLeaderboardRepository _leaderboard = new FakeLeaderboardRepository();
    private readonly FakeSessionService _sessions = new FakeSessionService();
    private readonly LeaderboardService _service;

    public TestLeaderboardService()
    {
        _service = new LeaderboardService(new FakeGameModeRepository(), _leaderboard, _sessions);
    }

    [Fact]
    public async Task GetLeaderboardAsync_UnknownMode_ReturnsUnknownGameMode()
    {
        var request = new LeaderboardRequest { GameModeCode = "CHESS" };

        LeaderboardPage page = await _service.GetLeaderboardAsync(request);

        Assert.Equal(LeaderboardResultCode.UnknownGameMode, page.Code);
    }

    [Fact]
    public async Task GetLeaderboardAsync_MoreRowsThanAPage_HasNextPage()
    {
        AddRows(LeaderboardRules.PageSize + 1);

        LeaderboardPage page = await _service.GetLeaderboardAsync(CreateRequest(0));

        Assert.True(page.HasNextPage);
    }

    [Fact]
    public async Task GetLeaderboardAsync_SecondPage_NumbersPositionsAfterTheFirstPage()
    {
        AddRows(LeaderboardRules.PageSize * 2);

        LeaderboardPage page = await _service.GetLeaderboardAsync(CreateRequest(1));

        Assert.Equal(LeaderboardRules.PageSize + 1, page.Entries.First().Position);
    }

    [Fact]
    public async Task GetLeaderboardAsync_PlayerWithTwoMatches_ReturnsMatchesToQualify()
    {
        SignIn(new ModeStatistic { MatchesPlayed = 2, Account = new Account { Nickname = "nico_nuevo" } });

        LeaderboardPage page = await _service.GetLeaderboardAsync(CreateSignedInRequest());

        Assert.Equal(LeaderboardRules.MinimumMatches - 2, page.OwnMatchesToQualify);
    }

    [Fact]
    public async Task GetLeaderboardAsync_RankedPlayer_ReturnsOwnPosition()
    {
        SignIn(new ModeStatistic { MatchesPlayed = 6, Account = new Account { Nickname = "luis_quo" } });
        _leaderboard.PlayersAhead = 4;

        LeaderboardPage page = await _service.GetLeaderboardAsync(CreateSignedInRequest());

        Assert.Equal(5, page.OwnEntry?.Position);
    }

    [Fact]
    public async Task GetLeaderboardAsync_NoSession_ReturnsNoOwnEntry()
    {
        AddRows(1);

        LeaderboardPage page = await _service.GetLeaderboardAsync(CreateRequest(0));

        Assert.Null(page.OwnEntry);
    }

    private void AddRows(int count)
    {
        for (int rowIndex = 0; rowIndex < count; rowIndex++)
        {
            _leaderboard.Rows.Add(new LeaderboardRow($"player_{rowIndex}", 1500, 10, 5));
        }
    }

    private void SignIn(ModeStatistic statistic)
    {
        _sessions.SignedInAccountId = OwnAccountId;
        _leaderboard.OwnStatistic = statistic;
    }

    private static LeaderboardRequest CreateRequest(int pageIndex)
    {
        return new LeaderboardRequest { GameModeCode = "CLASSIC", PageIndex = pageIndex };
    }

    private static LeaderboardRequest CreateSignedInRequest()
    {
        return new LeaderboardRequest { GameModeCode = "CLASSIC", SessionToken = FakeSessionService.IssuedToken };
    }
}
