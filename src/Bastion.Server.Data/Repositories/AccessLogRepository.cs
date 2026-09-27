using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public sealed class AccessLogRepository : IAccessLogRepository
{
    private readonly IDbContextFactory<BastionDbContext> _contextFactory;

    public AccessLogRepository(IDbContextFactory<BastionDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    public async Task AddAsync(AccessLog entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        context.AccessLogs.Add(entry);
        await context.SaveChangesAsync();
    }
}
