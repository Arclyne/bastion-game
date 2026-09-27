namespace Bastion.Client.Controls;

// The three boxes of a birth date, as the person typed them. They are parsed
// together because a day alone cannot be validated.
public sealed record BirthDateFields
{
    public required string Day { get; init; }

    public required string Month { get; init; }

    public required string Year { get; init; }
}
