namespace Bastion.Server.Services.Security;

public sealed record IssuedToken(string Token, byte[] TokenHash);
