using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Services.Sessions;

namespace Bastion.Server.Services.Tests.Fakes;

public sealed class FakeSessionService : ISessionService
{
    public const string IssuedToken = "token";

    public int? SignedInAccountId { get; set; }

    public Task<string> OpenAsync(int accountId, LoginRequest request)
    {
        return Task.FromResult(IssuedToken);
    }

    public Task<int?> FindAccountIdAsync(string token)
    {
        return Task.FromResult(token == IssuedToken ? SignedInAccountId : null);
    }
}
