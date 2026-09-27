using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

// Each operation opens its own short-lived context, so the repository can be shared by concurrent calls.
public sealed class AccountRepository : IAccountRepository
{
    private readonly IDbContextFactory<BastionDbContext> _contextFactory;

    public AccountRepository(IDbContextFactory<BastionDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    public async Task<Account?> FindByIdentifierAsync(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        string email = identifier.ToLowerInvariant();
        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.Nickname == identifier || account.Email == email);
    }

    public async Task<bool> IsNicknameTakenAsync(string nickname)
    {
        ArgumentNullException.ThrowIfNull(nickname);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts.AnyAsync(account => account.Nickname == nickname);
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts.AnyAsync(account => account.Email == email);
    }

    public async Task AddAsync(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
    }
}
