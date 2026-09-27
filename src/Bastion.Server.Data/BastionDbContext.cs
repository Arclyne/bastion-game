using System;
using Microsoft.EntityFrameworkCore;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data;

public sealed class BastionDbContext : DbContext
{
    public BastionDbContext(DbContextOptions<BastionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<GameMode> GameModes => Set<GameMode>();

    public DbSet<ModeStatistic> ModeStatistics => Set<ModeStatistic>();

    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BastionDbContext).Assembly);
    }
}
