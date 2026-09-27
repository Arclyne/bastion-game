using System;

namespace Bastion.Server.Data.Entities;

public class Session
{
    public int SessionId { get; set; }

    public int AccountId { get; set; }

    public byte[] TokenHash { get; set; } = Array.Empty<byte>();

    public DateTime StartedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime LastUsedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public string? CloseReason { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public string DeviceFingerprint { get; set; } = string.Empty;

    public string? DeviceName { get; set; }

    public string ClientVersion { get; set; } = string.Empty;
}
