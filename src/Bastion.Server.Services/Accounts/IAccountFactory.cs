using System.Threading.Tasks;
using Bastion.Contracts.Accounts;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Services.Accounts;

public interface IAccountFactory
{
    Task<Account> CreateAsync(RegistrationRequest request);
}
