using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public enum LoginResultCode
{
    [EnumMember]
    Success,

    [EnumMember]
    InvalidCredentials,

    [EnumMember]
    AccountLocked,

    [EnumMember]
    AccountPending,

    [EnumMember]
    AccountSuspended,

    [EnumMember]
    AccountBanned,

    [EnumMember]
    ServiceUnavailable,
}
