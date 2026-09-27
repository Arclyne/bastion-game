namespace Bastion.Client.Screens;

// The card measurements a screen hands to FormScreen. They travel together
// because the layout decides where the card sits and how tall it may be.
public sealed record CardShape
{
    public required int Width { get; init; }

    public required int Height { get; init; }

    public required ScreenLayout Layout { get; init; }
}
