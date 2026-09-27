using System.Collections.Generic;
using System.Globalization;
using Bastion.Client.Localization;
using Bastion.Client.Screens;
using Bastion.Client.Session;
using Bastion.Client.Tests.Fakes;
using Xunit;

namespace Bastion.Client.Tests;

public sealed class TestScreenSmoke
{
    public static TheoryData<ScreenId, string> ScreensInBothLanguages
    {
        get
        {
            var data = new TheoryData<ScreenId, string>();
            foreach (ScreenId screen in ScreenRegistry.RegisteredScreens)
            {
                data.Add(screen, Language.English.Name);
                data.Add(screen, Language.SpanishMexico.Name);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(ScreensInBothLanguages))]
    public void Create_RegisteredScreen_BuildsInEachLanguage(ScreenId screen, string cultureName)
    {
        Language.Apply(new CultureInfo(cultureName));
        var context = new ScreenContext(new FakeNavigator(), null, CreateServices());

        IScreen created = ScreenRegistry.Create(screen, context);

        Assert.NotNull(created);
    }

    private static ClientServices CreateServices()
    {
        return new ClientServices
        {
            Accounts = new FakeAccountClient(),
            Leaderboard = new FakeLeaderboardClient(),
            Session = new SessionContext(),
        };
    }
}
