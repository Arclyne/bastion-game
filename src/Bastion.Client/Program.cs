using System;
using System.IO;
using log4net;
using log4net.Config;
using Bastion.Client.Networking;
using Bastion.Client.Session;

namespace Bastion.Client;

public static class Program
{
    private const string LogConfigurationFileName = "log4net.config";

    public static void Main()
    {
        ConfigureLogging();

        using var accounts = new AccountClient();
        using var leaderboard = new LeaderboardClient();
        var services = new ClientServices
        {
            Accounts = accounts,
            Leaderboard = leaderboard,
            Session = new SessionContext(),
        };
        using var game = new BastionGame(services);
        game.Run();
    }

    private static void ConfigureLogging()
    {
        var configurationFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, LogConfigurationFileName));
        XmlConfigurator.Configure(LogManager.GetRepository(typeof(Program).Assembly), configurationFile);
    }
}
