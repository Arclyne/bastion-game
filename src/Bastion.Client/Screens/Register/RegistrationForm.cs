namespace Bastion.Client.Screens.Register;

public sealed class RegistrationForm
{
    public string Nickname { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Confirmation { get; set; } = string.Empty;

    public string Day { get; set; } = string.Empty;

    public string Month { get; set; } = string.Empty;

    public string Year { get; set; } = string.Empty;

    public bool HasAcceptedTerms { get; set; }
}
