using System.Runtime.Serialization;

namespace Bastion.Contracts.Leaderboard;

[DataContract]
public sealed class LeaderboardRequest
{
    [DataMember]
    public string GameModeCode { get; set; } = string.Empty;

    [DataMember]
    public int PageIndex { get; set; }

    [DataMember]
    public string? SessionToken { get; set; }
}
