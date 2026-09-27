using System;

namespace Bastion.Server.Data.Entities;

public class AccessLog
{
    public long AccessLogId { get; set; }

    public int? AccountId { get; set; }

    public string? EnteredIdentifier { get; set; }

    public AccessLogResult Result { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public DateTime OccurredAt { get; set; }
}
