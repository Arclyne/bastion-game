using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Repositories;

public sealed class GameModeRepository : IGameModeRepository
{
    private readonly IDbContextFactory<BastionDbContext> _contextFactory;

    public GameModeRepository(IDbContextFactory<BastionDbContext> contextFactory)
    {
        ArgumentNullException.ThrowIfNull(contextFactory);

        _contextFactory = contextFactory;
    }

    public async Task<IReadOnlyList<GameMode>> GetActiveAsync()
    {
        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.GameModes
            .AsNoTracking()
            .Where(gameMode => gameMode.IsActive)
            .ToListAsync();
    }

    public async Task<GameMode?> FindByCodeAsync(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        await using BastionDbContext context = await _contextFactory.CreateDbContextAsync();
        return await context.GameModes
            .AsNoTracking()
            .FirstOrDefaultAsync(gameMode => gameMode.Code == code);
    }
}
