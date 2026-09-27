using Bastion.Client.Screens;

namespace Bastion.Client.Tests.Fakes;

public sealed class FakeNavigator : INavigator
{
    public void GoTo(ScreenId screen, string? argument = null)
    {
    }

    public void GoBack()
    {
    }

    public void ReturnTo(ScreenId screen)
    {
    }

    public void Restart(ScreenId screen)
    {
    }

    public void ShowConfirm(ConfirmRequest request)
    {
    }

    public void ShowMessage(DialogTone tone, string body)
    {
    }
}
