using System.Runtime.Serialization;

namespace Bastion.Contracts.Leaderboard;

[DataContract]
public enum LeaderboardResultCode
{
    [EnumMember]
    Success,

    [EnumMember]
    UnknownGameMode,

    [EnumMember]
    ServiceUnavailable,
}
