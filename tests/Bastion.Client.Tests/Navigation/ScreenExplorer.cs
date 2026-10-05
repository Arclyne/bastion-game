using System.Collections.Generic;
using System.Linq;
using Bastion.Client.Controls;
using Bastion.Client.Screens;
using Bastion.Client.Session;
using Bastion.Client.Tests.Fakes;
using Bastion.Contracts.Accounts;

namespace Bastion.Client.Tests.Navigation;

public static class ScreenExplorer
{
    private static readonly IReadOnlyDictionary<ScreenId, string[][]> _typing = new Dictionary<ScreenId, string[][]>
    {
        [ScreenId.Login] = [["luis_quo", "Luis#Quo2026"]],
        [ScreenId.ForgotPassword] = [["luis.quo@example.test"]],
        [ScreenId.Register] = [["nora_north", "nora@example.test", "Nora#North26", "Nora#North26", "15", "3", "2008"]],
    };

    private static readonly LoginResultCode[] _loginAnswers =
    [
        LoginResultCode.Success,
        LoginResultCode.AccountPending,
        LoginResultCode.AccountBanned,
    ];

    private static readonly IReadOnlyDictionary<ScreenId, string?[]> _arguments = new Dictionary<ScreenId, string?[]>
    {
        [ScreenId.Match] = [null, ScreenArgument.AiOpponent],
        [ScreenId.MatchEnd] = [ScreenArgument.VictoryOutcome, ScreenArgument.DefeatOutcome],
        [ScreenId.WaitingRoom] = [ScreenArgument.HostRole, ScreenArgument.GuestRole],
    };

    public static IReadOnlyList<PressOutcome> Explore(ScreenId screen)
    {
        var outcomes = new List<PressOutcome>();
        foreach (Visit visit in GetVisits(screen))
        {
            outcomes.AddRange(ExploreVisit(visit));
        }

        return outcomes;
    }

    private static IEnumerable<Visit> GetVisits(ScreenId screen)
    {
        string?[] arguments = _arguments.TryGetValue(screen, out string?[]? values) ? values : [null];
        LoginResultCode[] answers = screen == ScreenId.Login ? _loginAnswers : [LoginResultCode.Success];
        return arguments.SelectMany(argument => answers.Select(answer => new Visit(screen, argument, answer)));
    }

    private static List<PressOutcome> ExploreVisit(Visit visit)
    {
        var outcomes = new List<PressOutcome>();
        int pressableCount = ScreenDriver.GetClickables(Open(visit, new RecordingNavigator())).Count;
        for (int index = 0; index < pressableCount; index++)
        {
            outcomes.Add(Press(visit, index, false));
        }

        IReadOnlyList<SidebarMenu> menus = ScreenDriver.GetMenus(Open(visit, new RecordingNavigator()));
        int entryCount = menus.Count == 0 ? 0 : menus[0].Items.Count;
        for (int index = 0; index < entryCount; index++)
        {
            outcomes.Add(Press(visit, index, true));
        }

        return outcomes;
    }

    private static PressOutcome Press(Visit visit, int index, bool isMenuEntry)
    {
        var navigator = new RecordingNavigator();
        IScreen screen = Open(visit, navigator);
        if (isMenuEntry)
        {
            ScreenDriver.ChooseMenuItem(screen, index);
        }
        else
        {
            ScreenDriver.ClickAt(screen, index);
        }

        screen.Update(new InputState());
        foreach (ConfirmRequest confirmation in navigator.Confirmations.ToList())
        {
            confirmation.OnConfirm();
        }

        return new PressOutcome(navigator.Destinations, navigator.HasGoneBack);
    }

    private static IScreen Open(Visit visit, RecordingNavigator navigator)
    {
        var accounts = new FakeAccountClient
        {
            LoginResult = new LoginResult { Code = visit.LoginAnswer, SessionToken = "token", Nickname = "luis_quo" },
            RegistrationResult = new RegistrationResult { Code = RegistrationResultCode.Registered },
        };
        var services = new ClientServices
        {
            Accounts = accounts,
            Leaderboard = new FakeLeaderboardClient(),
            Session = new SessionContext(),
        };
        IScreen screen = ScreenRegistry.Create(visit.Screen, new ScreenContext(navigator, visit.Argument, services));
        if (_typing.TryGetValue(visit.Screen, out string[][]? variants))
        {
            ScreenDriver.Type(screen, variants[0]);
            ScreenDriver.TickEveryBox(screen);
        }

        return screen;
    }

    private sealed record Visit(ScreenId Screen, string? Argument, LoginResultCode LoginAnswer);
}
