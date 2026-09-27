using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Configurations;

public sealed class ModeStatisticConfiguration : IEntityTypeConfiguration<ModeStatistic>
{
    public void Configure(EntityTypeBuilder<ModeStatistic> builder)
    {
        builder.ToTable(nameof(ModeStatistic));
        builder.HasKey(statistic => new { statistic.AccountId, statistic.GameModeId });
        builder.HasOne(statistic => statistic.GameMode)
            .WithMany()
            .HasForeignKey(statistic => statistic.GameModeId);
    }
}
