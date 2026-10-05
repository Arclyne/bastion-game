using System;
using System.IO;
using log4net;
using log4net.Config;
using Bastion.Client.Networking;
using Bastion.Client.Screens;
using Bastion.Client.Session;

namespace Bastion.Client;

public static class Program
{
    private const string LogConfigurationFileName = "log4net.config";

    // Opens the board on its own instead of the usual flow, to build and look at
    // the scene without going through the menus.
    private const string BoardSceneOption = "--board";

    public static void Main(string[] args)
    {
        ArgumentNullException.ThrowIfNull(args);

        ConfigureLogging();

        using var accounts = new AccountClient();
        using var leaderboard = new LeaderboardClient();
        var services = new ClientServices
        {
            Accounts = accounts,
            Leaderboard = leaderboard,
            Session = new SessionContext(),
        };
        using var game = new BastionGame(services, GetStartScreen(args));
        game.Run();
    }

    private static ScreenId GetStartScreen(string[] args)
    {
        return Array.IndexOf(args, BoardSceneOption) >= 0 ? ScreenId.BoardScene : ScreenId.MainScreen;
    }

    private static void ConfigureLogging()
    {
        var configurationFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, LogConfigurationFileName));
        XmlConfigurator.Configure(LogManager.GetRepository(typeof(Program).Assembly), configurationFile);
    }
}
