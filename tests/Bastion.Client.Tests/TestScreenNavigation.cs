using System.Collections.Generic;
using System.Linq;
using Bastion.Client.Localization;
using Bastion.Client.Screens;
using Bastion.Client.Tests.Navigation;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestScreenNavigation
{
    private const ScreenId Entry = ScreenId.MainScreen;

    private static readonly HashSet<ScreenId> _homeScreens = [ScreenId.MainScreen, ScreenId.Login, ScreenId.MainMenu];

    private static readonly HashSet<ScreenId> _matchExits = [ScreenId.MatchEnd, ScreenId.AIMatchEnd];

    private static readonly HashSet<ScreenId> _notYetReachable = [ScreenId.SecondFactor];

    private static readonly HashSet<ScreenId> _transitions = [ScreenId.VersusScreen];

    private readonly Dictionary<ScreenId, IReadOnlyList<PressOutcome>> _outcomes;

    public TestScreenNavigation()
    {
        Language.Apply(Language.English);
        _outcomes = ScreenRegistry.RegisteredScreens.ToDictionary(screen => screen, ScreenExplorer.Explore);
    }

    [Fact]
    public void Crawl_FromTheTitleScreen_ReachesEveryScreen()
    {
        HashSet<ScreenId> reached = Crawl();

        IEnumerable<ScreenId> unreachableScreens = ScreenRegistry.RegisteredScreens
            .Where(screen => !reached.Contains(screen) && !_notYetReachable.Contains(screen));
        string unreachable = string.Join(", ", unreachableScreens);

        Assert.Equal(string.Empty, unreachable);
    }

    [Fact]
    public void Explore_EveryScreen_HasAWayOut()
    {
        var screens = ScreenRegistry.RegisteredScreens
            .Where(screen => !_homeScreens.Contains(screen) && !_transitions.Contains(screen))
            .ToList();

        string deadEnds = string.Join(", ", screens.Where(screen => !HasWayOut(_outcomes[screen])));

        Assert.Equal(string.Empty, deadEnds);
    }

    private HashSet<ScreenId> Crawl()
    {
        var reached = new HashSet<ScreenId> { Entry };
        var pending = new Queue<ScreenId>([Entry]);
        while (pending.Count > 0)
        {
            foreach (ScreenId next in _outcomes[pending.Dequeue()].SelectMany(outcome => outcome.Destinations))
            {
                if (reached.Add(next))
                {
                    pending.Enqueue(next);
                }
            }
        }

        return reached;
    }

    private static bool HasWayOut(IReadOnlyList<PressOutcome> outcomes)
    {
        return outcomes.Any(outcome => outcome.HasGoneBack || outcome.Destinations.Any(IsExit));
    }

    private static bool IsExit(ScreenId screen)
    {
        return _homeScreens.Contains(screen) || _matchExits.Contains(screen);
    }
}
