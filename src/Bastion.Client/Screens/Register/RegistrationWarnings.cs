namespace Bastion.Client.Screens.Register;

public sealed class RegistrationWarnings
{
    public string? Nickname { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Confirmation { get; set; }

    public string? BirthDate { get; set; }

    public string? Terms { get; set; }

    public bool HasAny => Nickname is not null
        || Email is not null
        || Password is not null
        || Confirmation is not null
        || BirthDate is not null
        || Terms is not null;
}
