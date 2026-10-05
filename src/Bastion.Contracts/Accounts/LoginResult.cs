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

    // Only sent for an account pending verification, after the password was checked (CU-01 FA-09).
    [DataMember]
    public string? Email { get; set; }
}
