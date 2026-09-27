using System;

namespace Bastion.Server.Services.Common;

public interface ICallContext
{
    DateTime UtcNow { get; }

    string ClientAddress { get; }
}
