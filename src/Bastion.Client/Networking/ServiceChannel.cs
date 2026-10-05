using System;
using System.ServiceModel;
using System.Threading.Tasks;
using log4net;

namespace Bastion.Client.Networking;

public sealed class ServiceChannel<TContract> : IDisposable
    where TContract : class
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(ServiceChannel<TContract>));

    private readonly ChannelFactory<TContract> _factory;

    public ServiceChannel(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var binding = new NetTcpBinding(SecurityMode.None);
        _factory = new ChannelFactory<TContract>(binding, new EndpointAddress(ServerAddress.Create(path)));
    }

    public async Task<TResult?> CallAsync<TResult>(Func<TContract, Task<TResult>> operation)
        where TResult : class
    {
        ArgumentNullException.ThrowIfNull(operation);

        TContract channel = _factory.CreateChannel();
        var communicationObject = (ICommunicationObject)channel;
        try
        {
            TResult result = await operation(channel);
            communicationObject.Close();
            return result;
        }
        catch (CommunicationException ex)
        {
            communicationObject.Abort();
            _logger.Warn($"The server could not be reached. Contract={typeof(TContract).Name}", ex);
            return null;
        }
        catch (TimeoutException ex)
        {
            communicationObject.Abort();
            _logger.Warn($"The server did not answer in time. Contract={typeof(TContract).Name}", ex);
            return null;
        }
    }

    public void Dispose()
    {
        if (_factory.State == CommunicationState.Faulted)
        {
            _factory.Abort();
            return;
        }

        _factory.Close();
    }
}
