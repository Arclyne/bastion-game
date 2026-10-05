using System;

namespace Bastion.Client.Screens;

public static class Popup
{
    public static void ShowError(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Error, body);
    }

    public static void ShowAlert(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Warning, body);
    }

    public static void ShowNotice(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Success, body);
    }

    public static void Ask(INavigator navigator, ConfirmRequest request)
    {
        ArgumentNullException.ThrowIfNull(navigator);
        ArgumentNullException.ThrowIfNull(request);

        navigator.ShowConfirm(request);
    }

    private static void Show(INavigator navigator, DialogTone tone, string body)
    {
        ArgumentNullException.ThrowIfNull(navigator);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        navigator.ShowMessage(tone, body);
    }
}
