using Bastion.Client.Localization;
using Bastion.Client.Screens;
using Bastion.Client.Screens.Messages;
using Bastion.Client.Session;
using Bastion.Client.Tests.Fakes;
using Bastion.Client.Tests.Navigation;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestNavigator
{
    private readonly Navigator _navigator;

    public TestNavigator()
    {
        Language.Apply(Language.English);
        _navigator = new Navigator(new ClientServices
        {
            Accounts = new FakeAccountClient(),
            Leaderboard = new FakeLeaderboardClient(),
            Session = new SessionContext(),
        });
        _navigator.Start(ScreenId.MainScreen);
    }

    [Fact]
    public void GoBack_AfterGoTo_ReturnsToThePreviousScreen()
    {
        _navigator.GoTo(ScreenId.Login);

        _navigator.GoBack();

        Assert.Equal(ScreenId.MainScreen, _navigator.CurrentId);
    }

    [Fact]
    public void GoBack_TwoScreensDeep_ReturnsOneStep()
    {
        _navigator.GoTo(ScreenId.Login);
        _navigator.GoTo(ScreenId.Register);

        _navigator.GoBack();

        Assert.Equal(ScreenId.Login, _navigator.CurrentId);
    }

    [Fact]
    public void GoBack_AfterRestart_StaysOnTheRestartedScreen()
    {
        _navigator.GoTo(ScreenId.Login);
        _navigator.Restart(ScreenId.MainMenu);

        _navigator.GoBack();

        Assert.Equal(ScreenId.MainMenu, _navigator.CurrentId);
    }

    [Fact]
    public void ReturnTo_ScreenInHistory_DropsTheScreensAfterIt()
    {
        _navigator.Restart(ScreenId.MainMenu);
        _navigator.GoTo(ScreenId.AccountSettings);
        _navigator.GoTo(ScreenId.ChangePassword);

        _navigator.ReturnTo(ScreenId.MainMenu);

        Assert.Equal(ScreenId.MainMenu, _navigator.CurrentId);
    }

    [Fact]
    public void CancelRegistration_Confirmed_ReturnsToLogin()
    {
        _navigator.GoTo(ScreenId.Login);
        _navigator.GoTo(ScreenId.Register);
        ScreenDriver.ClickButton(ScreenOf(_navigator), TextCatalog.RegisterCancelButton);

        ScreenDriver.ConfirmDialog((MessageScreen)ScreenOf(_navigator));

        Assert.Equal(ScreenId.Login, _navigator.CurrentId);
    }

    private static IScreen ScreenOf(Navigator navigator)
    {
        return navigator.Current ?? throw new Xunit.Sdk.XunitException("The navigator has no screen open.");
    }
}
