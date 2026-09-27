using System.Threading.Tasks;
using Bastion.Contracts.Accounts;

namespace Bastion.Server.Services.Accounts;

public interface IRegistrationValidator
{
    Task<RegistrationResultCode?> FindProblemAsync(RegistrationRequest request);
}
