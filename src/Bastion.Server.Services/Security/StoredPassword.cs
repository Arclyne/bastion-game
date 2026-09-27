namespace Bastion.Server.Services.Security;

public sealed record StoredPassword(byte[] Hash, byte[] Salt);
