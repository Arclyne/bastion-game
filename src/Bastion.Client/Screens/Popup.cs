using System;

namespace Bastion.Client.Screens;

// The one place a screen asks for a message over what it is showing. The
// navigator already knows how to draw a dialog; this names the three tones so
// a screen never has to pick a DialogTone by hand, and so every screen says
// the same kind of thing the same way.
public static class Popup
{
    // Something went wrong and the person cannot fix it by typing again.
    public static void ShowError(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Error, body);
    }

    // Something is wrong and the person can fix it: the form is waiting.
    public static void ShowAlert(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Warning, body);
    }

    // Nothing is wrong; the screen is telling the person what just happened.
    public static void ShowNotice(INavigator navigator, string body)
    {
        Show(navigator, DialogTone.Success, body);
    }

    // A question with two answers. The request carries the labels, so a
    // screen that asks something unusual does not need a new method here.
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
