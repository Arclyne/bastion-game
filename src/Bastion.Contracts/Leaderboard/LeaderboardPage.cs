using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bastion.Contracts.Leaderboard;

[DataContract]
public sealed class LeaderboardPage
{
    [DataMember]
    public LeaderboardResultCode Code { get; set; }

    [DataMember]
    public List<LeaderboardEntry> Entries { get; set; } = new List<LeaderboardEntry>();

    [DataMember]
    public bool HasNextPage { get; set; }

    [DataMember]
    public LeaderboardEntry? OwnEntry { get; set; }

    [DataMember]
    public int OwnMatchesToQualify { get; set; }
}
