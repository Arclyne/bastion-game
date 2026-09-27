using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bastion.Server.Data.Conversions;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Configurations;

public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    private const int IdentifierLength = 254;
    private const int ResultLength = 40;
    private const int IpAddressLength = 45;

    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable(nameof(AccessLog));
        builder.HasKey(entry => entry.AccessLogId);
        builder.Property(entry => entry.EnteredIdentifier).HasMaxLength(IdentifierLength);
        builder.Property(entry => entry.Result)
            .HasConversion(new UpperSnakeCaseEnumConverter<AccessLogResult>())
            .HasMaxLength(ResultLength)
            .IsUnicode(false);
        builder.Property(entry => entry.IpAddress).HasMaxLength(IpAddressLength).IsUnicode(false);
    }
}
