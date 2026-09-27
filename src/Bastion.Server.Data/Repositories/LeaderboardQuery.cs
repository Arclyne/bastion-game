namespace Bastion.Server.Data.Repositories;

public sealed class LeaderboardQuery
{
    public byte GameModeId { get; set; }

    public int MinimumMatches { get; set; }

    public int Skip { get; set; }

    public int Take { get; set; }
}
