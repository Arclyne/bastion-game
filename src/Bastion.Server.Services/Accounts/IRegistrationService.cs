using System.Threading.Tasks;
using Bastion.Contracts.Accounts;

namespace Bastion.Server.Services.Accounts;

public interface IRegistrationService
{
    Task<RegistrationOutcome> RegisterAsync(RegistrationRequest request);
}
