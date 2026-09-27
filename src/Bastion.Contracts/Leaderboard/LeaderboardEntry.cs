using System.Runtime.Serialization;

namespace Bastion.Contracts.Leaderboard;

[DataContract]
public sealed class LeaderboardEntry
{
    [DataMember]
    public int Position { get; set; }

    [DataMember]
    public string Nickname { get; set; } = string.Empty;

    [DataMember]
    public int EloRating { get; set; }

    [DataMember]
    public int MatchesPlayed { get; set; }

    [DataMember]
    public int MatchesWon { get; set; }
}
