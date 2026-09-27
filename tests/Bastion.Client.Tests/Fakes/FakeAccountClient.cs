using System.Threading.Tasks;
using Bastion.Client.Networking;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Tests.Fakes;

public sealed class FakeAccountClient : IAccountClient
{
    public LoginResult LoginResult { get; set; } = new LoginResult { Code = LoginResultCode.InvalidCredentials };

    public RegistrationResult RegistrationResult { get; set; } = new RegistrationResult();

    public RegistrationRequest? LastRegistration { get; private set; }

    public Task<LoginResult> LogInAsync(LoginRequest request)
    {
        return Task.FromResult(LoginResult);
    }

    public Task<RegistrationResult> RegisterAsync(RegistrationRequest request)
    {
        LastRegistration = request;
        return Task.FromResult(RegistrationResult);
    }
}
