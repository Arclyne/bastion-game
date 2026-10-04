using System;
using System.Collections.Generic;
using Bastion.Client.Screens;

namespace Bastion.Client.Tests.Navigation;

// Records where a screen asks to go instead of going there, so every press can be checked on its own.
public sealed class RecordingNavigator : INavigator
{
    public List<ScreenId> Destinations { get; } = [];

    public List<ConfirmRequest> Confirmations { get; } = [];

    public bool HasGoneBack { get; private set; }

    public bool HasShownMessage { get; private set; }

    public bool CanGoBack => true;

    public void GoTo(ScreenId screen, string? argument = null)
    {
        Destinations.Add(screen);
    }

    public void GoBack()
    {
        HasGoneBack = true;
    }

    public void ReturnTo(ScreenId screen)
    {
        Destinations.Add(screen);
    }

    public void Restart(ScreenId screen)
    {
        Destinations.Add(screen);
    }

    public void ShowConfirm(ConfirmRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Confirmations.Add(request);
    }

    public void ShowMessage(DialogTone tone, string body)
    {
        HasShownMessage = true;
    }
}
