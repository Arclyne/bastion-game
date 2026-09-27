using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

// A wrong identifier and a wrong password share InvalidCredentials on purpose (CU-01 RN-01).
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
