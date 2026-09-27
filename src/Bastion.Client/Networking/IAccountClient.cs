using System.Threading.Tasks;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Networking;

public interface IAccountClient
{
    Task<LoginResult> LogInAsync(LoginRequest request);

    Task<RegistrationResult> RegisterAsync(RegistrationRequest request);
}
