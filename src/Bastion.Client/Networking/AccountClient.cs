using System;
using System.Threading.Tasks;
using Bastion.Contracts;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Networking;

public sealed class AccountClient : IAccountClient, IDisposable
{
    private readonly ServiceChannel<IAccountService> _channel = new ServiceChannel<IAccountService>(
        ServiceEndpoints.AccountsPath);

    public async Task<LoginResult> LogInAsync(LoginRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LoginResult? result = await _channel.CallAsync(service => service.LogInAsync(request));
        return result ?? new LoginResult { Code = LoginResultCode.ServiceUnavailable };
    }

    public async Task<RegistrationResult> RegisterAsync(RegistrationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        RegistrationResult? result = await _channel.CallAsync(service => service.RegisterAsync(request));
        return result ?? new RegistrationResult { Code = RegistrationResultCode.ServiceUnavailable };
    }

    public void Dispose()
    {
        _channel.Dispose();
    }
}
