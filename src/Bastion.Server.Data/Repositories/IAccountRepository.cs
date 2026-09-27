using System.Threading.Tasks;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public interface IAccountRepository
{
    Task<Account?> FindByIdentifierAsync(string identifier);

    Task<bool> IsNicknameTakenAsync(string nickname);

    Task<bool> IsEmailTakenAsync(string email);

    Task AddAsync(Account account);

    Task UpdateAsync(Account account);
}
