using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public sealed class LoginRequest
{
    [DataMember]
    public string Identifier { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;

    [DataMember]
    public string DeviceFingerprint { get; set; } = string.Empty;

    [DataMember]
    public string ClientVersion { get; set; } = string.Empty;
}
