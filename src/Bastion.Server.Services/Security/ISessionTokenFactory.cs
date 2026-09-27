namespace Bastion.Server.Services.Security;

public interface ISessionTokenFactory
{
    IssuedToken Create();

    byte[] ComputeHash(string token);
}
