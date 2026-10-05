namespace Bastion.Client.Screens;

public interface INavigator
{
    void GoTo(ScreenId screen, string? argument = null);

    void GoBack();

    void ReturnTo(ScreenId screen);

    void Restart(ScreenId screen);

    void ShowConfirm(ConfirmRequest request);

    void ShowMessage(DialogTone tone, string body);
}
