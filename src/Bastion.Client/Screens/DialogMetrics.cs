using Bastion.Client.Controls;

namespace Bastion.Client.Screens;

public sealed record DialogMetrics
{
    public required TextStyle TitleStyle { get; init; }

    public required TextStyle BodyStyle { get; init; }

    public required TextStyle DetailStyle { get; init; }

    public required float TitleHeight { get; init; }

    public required float BodyHeight { get; init; }

    public required float DetailHeight { get; init; }

    public required float TotalHeight { get; init; }
}
