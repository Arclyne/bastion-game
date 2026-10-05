using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public sealed class LoginResult
{
    [DataMember]
    public LoginResultCode Code { get; set; }

    [DataMember]
    public string? SessionToken { get; set; }

    [DataMember]
    public string? Nickname { get; set; }

    [DataMember]
    public string? PreferredLanguage { get; set; }

    [DataMember]
    public int LockMinutes { get; set; }

    [DataMember]
    public string? Email { get; set; }
}
