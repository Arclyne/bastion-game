namespace Bastion.Client.Screens;

// Screens depend on this and never on each other, so adding a screen does not
// touch the ones that lead to it (rule 10.1).
public interface INavigator
{
    void GoTo(ScreenId screen, string? argument = null);

    bool CanGoBack { get; }

    void GoBack();

    void ReturnTo(ScreenId screen);

    void Restart(ScreenId screen);

    void ShowConfirm(ConfirmRequest request);

    void ShowMessage(DialogTone tone, string body);
}
