using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public sealed class RegistrationResult
{
    [DataMember]
    public RegistrationResultCode Code { get; set; }
}
