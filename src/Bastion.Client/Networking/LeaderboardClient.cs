using System;
using System.Threading.Tasks;
using Bastion.Contracts;
using Bastion.Contracts.Leaderboard;

namespace Bastion.Client.Networking;

public sealed class LeaderboardClient : ILeaderboardClient, IDisposable
{
    private readonly ServiceChannel<ILeaderboardService> _channel = new ServiceChannel<ILeaderboardService>(
        ServiceEndpoints.LeaderboardPath);

    public async Task<LeaderboardPage> GetLeaderboardAsync(LeaderboardRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LeaderboardPage? page = await _channel.CallAsync(service => service.GetLeaderboardAsync(request));
        return page ?? new LeaderboardPage { Code = LeaderboardResultCode.ServiceUnavailable };
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
