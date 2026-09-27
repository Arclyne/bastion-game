using System.Threading.Tasks;

namespace Bastion.Server.Services.Accounts;

public interface IAuthenticationService
{
    Task<AuthenticationOutcome> AuthenticateAsync(string identifier, string password);
}
