using System.Threading.Tasks;
using Bastion.Contracts.Accounts;

namespace Bastion.Server.Services.Accounts;

public interface ILoginService
{
    Task<LoginOutcome> LogInAsync(LoginRequest request);
}
