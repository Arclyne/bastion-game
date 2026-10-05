using System;
using System.IO;
using CoreWCF;
using CoreWCF.Configuration;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bastion.Contracts;
using Bastion.Contracts.Accounts;
using Bastion.Contracts.Leaderboard;
using Bastion.Server.Data;
using Bastion.Server.Services.Accounts;
using Bastion.Server.Services.Leaderboard;

namespace Bastion.Server.Host;

public static class Program
{
    private const string ConnectionStringName = "Bastion";
    private const string NetTcpPortKey = "Server:NetTcpPort";
    private const string LogConfigurationFileName = "log4net.config";
    private const string RelativeAddressPrefix = "/";

    private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        ConfigureLogging();
        // The content root is the build output, where appsettings.json is copied, so the server starts the same way
        // from any working directory (for example with dotnet run --project).
        var options = new WebApplicationOptions { Args = args, ContentRootPath = AppContext.BaseDirectory };
        WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

        // User secrets load in every environment, not only Development, so a developer machine never needs the
        // connection string in a file inside the repository.
        builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
        string connectionString = GetConnectionString(builder.Configuration);
        int netTcpPort = GetNetTcpPort(builder.Configuration);

        builder.Logging.ClearProviders();
        builder.Logging.AddLog4Net(new Log4NetProviderOptions { ExternalConfigurationSetup = true });
        builder.WebHost.UseNetTcp(netTcpPort);
        builder.Services.AddServiceModelServices();
        builder.Services.AddDbContextFactory<BastionDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddBastionRepositories();
        builder.Services.AddBastionServices();

        WebApplication app = builder.Build();
        app.UseServiceModel(RegisterServices);
        _logger.Info($"Game server started. NetTcpPort={netTcpPort}");
        app.Run();
    }

    private static void ConfigureLogging()
    {
        var configurationFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, LogConfigurationFileName));
        XmlConfigurator.Configure(LogManager.GetRepository(typeof(Program).Assembly), configurationFile);
    }

    // The connection string lives in user-secrets or in the ConnectionStrings__Bastion environment variable,
    // never in the repository.
    private static string GetConnectionString(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(ConnectionStringName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"The connection string is not configured. ConnectionStringName={ConnectionStringName}");
        }

        return connectionString;
    }

    private static int GetNetTcpPort(IConfiguration configuration)
    {
        int netTcpPort = configuration.GetValue<int>(NetTcpPortKey);
        if (netTcpPort <= 0)
        {
            throw new InvalidOperationException($"The net.tcp port is not configured. Key={NetTcpPortKey}");
        }

        return netTcpPort;
    }

    private static void RegisterServices(IServiceBuilder serviceBuilder)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);

        serviceBuilder.AddService<AccountService>();
        serviceBuilder.AddServiceEndpoint<AccountService, IAccountService>(
            CreateBinding(),
            ToRelativeAddress(ServiceEndpoints.AccountsPath));
        serviceBuilder.AddService<LeaderboardService>();
        serviceBuilder.AddServiceEndpoint<LeaderboardService, ILeaderboardService>(
            CreateBinding(),
            ToRelativeAddress(ServiceEndpoints.LeaderboardPath));
    }

    // SecurityMode.None is required between macOS and Linux; it is compensated by server-side validation, hashed
    // passwords and session tokens (see the stack decisions in the README).
    private static NetTcpBinding CreateBinding()
    {
        return new NetTcpBinding(SecurityMode.None);
    }

    private static string ToRelativeAddress(string path)
    {
        return RelativeAddressPrefix + path;
    }
}
