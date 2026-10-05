namespace Bastion.Client.Controls;

public sealed record RoundedRectangleShape
{
    public required int Width { get; init; }

    public required int Height { get; init; }

    public required int CornerRadius { get; init; }

    public int BorderThickness { get; init; }
}
