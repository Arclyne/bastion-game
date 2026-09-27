namespace Bastion.Client.Controls;

// Cache key for a generated rounded rectangle texture. A record gives value
// equality for free, which is what the texture cache needs.
public sealed record RoundedRectangleShape
{
    public required int Width { get; init; }

    public required int Height { get; init; }

    public required int CornerRadius { get; init; }

    // Zero produces a solid shape; a greater value produces only the inner ring.
    public int BorderThickness { get; init; }
}
