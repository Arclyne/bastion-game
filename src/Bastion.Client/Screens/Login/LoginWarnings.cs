namespace Bastion.Client.Screens.Login;

public sealed record LoginWarnings(string? Identifier, string? Password)
{
    public bool HasAny => Identifier is not null || Password is not null;
}
