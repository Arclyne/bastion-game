using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bastion.Server.Data.Entities;

namespace Bastion.Server.Data.Configurations;

public sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    private const int TokenHashLength = 32;
    private const int CloseReasonLength = 20;
    private const int IpAddressLength = 45;
    private const int DeviceFingerprintLength = 128;
    private const int DeviceNameLength = 100;
    private const int ClientVersionLength = 20;

    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.ToTable(nameof(Session));
        builder.HasKey(session => session.SessionId);
        builder.Property(session => session.TokenHash).HasMaxLength(TokenHashLength);
        builder.Property(session => session.CloseReason).HasMaxLength(CloseReasonLength).IsUnicode(false);
        builder.Property(session => session.IpAddress).HasMaxLength(IpAddressLength).IsUnicode(false);
        builder.Property(session => session.DeviceFingerprint).HasMaxLength(DeviceFingerprintLength).IsUnicode(false);
        builder.Property(session => session.DeviceName).HasMaxLength(DeviceNameLength);
        builder.Property(session => session.ClientVersion).HasMaxLength(ClientVersionLength).IsUnicode(false);
    }
}
