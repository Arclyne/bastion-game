using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Configurations;

public sealed class GameModeConfiguration : IEntityTypeConfiguration<GameMode>
{
    private const int CodeLength = 40;

    public void Configure(EntityTypeBuilder<GameMode> builder)
    {
        builder.ToTable(nameof(GameMode));
        builder.HasKey(gameMode => gameMode.GameModeId);
        builder.Property(gameMode => gameMode.GameModeId).ValueGeneratedNever();
        builder.Property(gameMode => gameMode.Code).HasMaxLength(CodeLength).IsUnicode(false);
    }
}
