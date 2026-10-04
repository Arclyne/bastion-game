using Bastion.Client;
using Bastion.Client.Localization;
using Bastion.Client.Screens;
using Bastion.Client.Session;
using Bastion.Client.Tests.Fakes;
using Xunit;

namespace Bastion.Client.Tests;

// The board scene is opened with --board, so it starts with no history behind
// it. Going back from there would rebuild the scene and throw away the angle the
// player turned the board to.
public sealed class TestBoardSceneNavigation
{
    private readonly Navigator _navigator;

    public TestBoardSceneNavigation()
    {
        Language.Apply(Language.English);
        _navigator = new Navigator(new ClientServices
        {
            Accounts = new FakeAccountClient(),
            Leaderboard = new FakeLeaderboardClient(),
            Session = new SessionContext(),
        });
        _navigator.Start(ScreenId.BoardScene);
    }

    [Fact]
    public void CanGoBack_ASceneOpenedOnItsOwn_IsFalse()
    {
        bool canGoBack = _navigator.CanGoBack;

        Assert.False(canGoBack);
    }

    [Fact]
    public void CanGoBack_AfterReachingASecondScreen_IsTrue()
    {
        _navigator.GoTo(ScreenId.MainMenu);

        Assert.True(_navigator.CanGoBack);
    }

    // The world behind a dialog has to keep being drawn, so the screen stays
    // reachable even while the dialog is the one on top.
    [Fact]
    public void CurrentScreen_WithADialogOnTop_IsStillTheSceneUnderneath()
    {
        IScreen? scene = _navigator.CurrentScreen;

        _navigator.ShowMessage(DialogTone.Warning, string.Empty);

        Assert.Same(scene, _navigator.CurrentScreen);
    }

    [Fact]
    public void Current_WithADialogOnTop_IsTheDialogAndNotTheScene()
    {
        IScreen? scene = _navigator.CurrentScreen;

        _navigator.ShowMessage(DialogTone.Warning, string.Empty);

        Assert.NotSame(scene, _navigator.Current);
    }
}
