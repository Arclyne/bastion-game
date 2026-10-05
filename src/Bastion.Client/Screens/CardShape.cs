namespace Bastion.Client.Screens;

public sealed record CardShape
{
    public required int Width { get; init; }

    public required int Height { get; init; }

    public required ScreenLayout Layout { get; init; }
}
