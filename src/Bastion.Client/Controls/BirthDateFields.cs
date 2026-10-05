namespace Bastion.Client.Controls;

public sealed record BirthDateFields
{
    public required string Day { get; init; }

    public required string Month { get; init; }

    public required string Year { get; init; }
}
