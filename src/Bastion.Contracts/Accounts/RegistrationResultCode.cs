using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public enum RegistrationResultCode
{
    [EnumMember]
    Registered,

    [EnumMember]
    NicknameTaken,

    [EnumMember]
    EmailTaken,

    [EnumMember]
    InvalidData,

    [EnumMember]
    Underage,

    [EnumMember]
    TermsNotAccepted,

    [EnumMember]
    ServiceUnavailable,
}
