using System;
using System.Runtime.Serialization;

namespace Bastion.Contracts.Accounts;

[DataContract]
public sealed class RegistrationRequest
{
    [DataMember]
    public string Nickname { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;

    [DataMember]
    public DateTime BirthDate { get; set; }

    [DataMember]
    public string PreferredLanguage { get; set; } = string.Empty;

    [DataMember]
    public bool HasAcceptedTerms { get; set; }
}
