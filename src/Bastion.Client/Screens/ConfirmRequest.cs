using System;

namespace Bastion.Client.Screens;

// What a screen needs to say to ask for a confirmation. The labels are optional
// because most confirmations use the generic pair from the catalog.
public sealed record ConfirmRequest
{
    public required string Body { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public required Action OnConfirm { get; init; }

    public string PrimaryLabel { get; init; } = string.Empty;

    public string SecondaryLabel { get; init; } = string.Empty;
}
