using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bastion.Server.Data.Entities;
using Bastion.Server.Data.Repositories;

namespace Bastion.Server.Services.Tests.Fakes;

public sealed class FakeAccountRepository : IAccountRepository
{
    public List<Account> Accounts { get; } = [];

    public Task<Account?> FindByIdentifierAsync(string identifier)
    {
        Account? account = Accounts.FirstOrDefault(candidate => IsIdentifiedBy(candidate, identifier));
        return Task.FromResult(account);
    }

    public Task<bool> IsNicknameTakenAsync(string nickname)
    {
        return Task.FromResult(Accounts.Any(account => account.Nickname == nickname));
    }

    public Task<bool> IsEmailTakenAsync(string email)
    {
        return Task.FromResult(Accounts.Any(account => account.Email == email));
    }

    public Task AddAsync(Account account)
    {
        Accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Account account)
    {
        return Task.CompletedTask;
    }

    private static bool IsIdentifiedBy(Account account, string identifier)
    {
        return account.Nickname == identifier
            || string.Equals(account.Email, identifier, StringComparison.OrdinalIgnoreCase);
    }
}
