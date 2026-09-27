using System;
using Microsoft.EntityFrameworkCore;

namespace Bastion.Server.Data;

public sealed class BastionDbContext : DbContext
{
    public BastionDbContext(DbContextOptions<BastionDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BastionDbContext).Assembly);
    }
}
