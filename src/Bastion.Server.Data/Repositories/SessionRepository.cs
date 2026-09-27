using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public sealed class SessionRepository : ISessionRepository
{
    private readonly IDbContextFactory<BastionDbContext> _contextFactory;

    public SessionRepository(IDbContextFactory<BastionDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    public async Task AddAsync(Session session)
    {
        ArgumentNullException.ThrowIfNull(session);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
    }

    public async Task<Session?> FindOpenAsync(byte[] tokenHash, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(tokenHash);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(session => session.TokenHash == tokenHash
                && session.EndedAt == null
                && session.ExpiresAt > now);
    }
}
