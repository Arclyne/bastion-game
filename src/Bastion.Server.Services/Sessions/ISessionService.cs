using System.Threading.Tasks;
using Bastion.Contracts.Accounts;

namespace Bastion.Server.Services.Sessions;

public interface ISessionService
{
    Task<string> OpenAsync(int accountId, LoginRequest request);

    Task<int?> FindAccountIdAsync(string token);
}
