namespace Bastion.Server.Services.Security;

public interface IPasswordHasher
{
    StoredPassword Hash(string password);

    bool Matches(string password, StoredPassword stored);
}
