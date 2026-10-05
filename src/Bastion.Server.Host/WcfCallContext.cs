using System;
using CoreWCF;
using CoreWCF.Channels;
using Bastion.Server.Services.Common;

namespace Bastion.Server.Host;

public sealed class WcfCallContext : ICallContext
{
    private const string UnknownAddress = "0.0.0.0";

    public DateTime UtcNow => DateTime.UtcNow;

    public string ClientAddress => GetClientAddress();

    private static string GetClientAddress()
    {
        MessageProperties? properties = OperationContext.Current?.IncomingMessageProperties;
        if (properties is null
            || !properties.TryGetValue(RemoteEndpointMessageProperty.Name, out object? value)
            || value is not RemoteEndpointMessageProperty endpoint)
        {
            return UnknownAddress;
        }

        return endpoint.Address;
    }
}
