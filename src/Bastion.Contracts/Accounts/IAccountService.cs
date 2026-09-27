using System.ServiceModel;
using System.Threading.Tasks;

namespace Bastion.Contracts.Accounts;

[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    Task<LoginResult> LogInAsync(LoginRequest request);

    [OperationContract]
    Task<RegistrationResult> RegisterAsync(RegistrationRequest request);
}
