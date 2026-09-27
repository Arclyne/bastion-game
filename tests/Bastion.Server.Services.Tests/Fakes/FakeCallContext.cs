using System;
using Bastion.Server.Services.Common;

namespace Bastion.Server.Services.Tests.Fakes;

public sealed class FakeCallContext : ICallContext
{
    public DateTime UtcNow { get; set; } = new DateTime(2026, 9, 27, 12, 0, 0, DateTimeKind.Utc);

    public string ClientAddress { get; set; } = "127.0.0.1";
}
