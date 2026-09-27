namespace Bastion.Client.Screens;

public sealed record ScreenContext(INavigator Navigator, string? Argument, ClientServices Services);
