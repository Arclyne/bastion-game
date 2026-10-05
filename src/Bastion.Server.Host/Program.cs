using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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
    private const string CertificatePathKey = "Server:CertificatePath";
    private const string CertificatePasswordKey = "Server:CertificatePassword";

    private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));

    public static void Main(string[] args)
    {
        ConfigureLogging();
        var options = new WebApplicationOptions { Args = args, ContentRootPath = AppContext.BaseDirectory };
        WebApplicationBuilder builder = WebApplication.CreateBuilder(options);

        builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);
        string connectionString = GetConnectionString(builder.Configuration);
        int netTcpPort = GetNetTcpPort(builder.Configuration);
        X509Certificate2 certificate = LoadCertificate(builder.Configuration);

        builder.Logging.ClearProviders();
        builder.Logging.AddLog4Net(new Log4NetProviderOptions { ExternalConfigurationSetup = true });
        builder.WebHost.UseNetTcp(netTcpPort);
        builder.Services.AddServiceModelServices();
        builder.Services.AddDbContextFactory<BastionDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddBastionRepositories();
        builder.Services.AddBastionServices();

        WebApplication app = builder.Build();
        app.UseServiceModel(serviceBuilder => RegisterServices(serviceBuilder, certificate));
        _logger.Info($"Game server started. NetTcpPort={netTcpPort}");
        app.Run();
    }

    private static void ConfigureLogging()
    {
        var configurationFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, LogConfigurationFileName));
        XmlConfigurator.Configure(LogManager.GetRepository(typeof(Program).Assembly), configurationFile);
    }

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

    private static X509Certificate2 LoadCertificate(IConfiguration configuration)
    {
        string? path = configuration[CertificatePathKey];
        string? password = configuration[CertificatePasswordKey];
        if (string.IsNullOrWhiteSpace(path) || password is null)
        {
            throw new InvalidOperationException(
                $"The server certificate is not configured. Key={CertificatePathKey}");
        }

        return X509CertificateLoader.LoadPkcs12FromFile(path, password);
    }

    private static void RegisterServices(IServiceBuilder serviceBuilder, X509Certificate2 certificate)
    {
        ArgumentNullException.ThrowIfNull(serviceBuilder);
        ArgumentNullException.ThrowIfNull(certificate);

        serviceBuilder.AddService<AccountService>();
        serviceBuilder.AddServiceEndpoint<AccountService, IAccountService>(
            CreateBinding(),
            ToRelativeAddress(ServiceEndpoints.AccountsPath));
        serviceBuilder.AddService<LeaderboardService>();
        serviceBuilder.AddServiceEndpoint<LeaderboardService, ILeaderboardService>(
            CreateBinding(),
            ToRelativeAddress(ServiceEndpoints.LeaderboardPath));
        serviceBuilder.ConfigureAllServiceHostBase(
            host => host.Credentials.ServiceCertificate.Certificate = certificate);
    }

    private static NetTcpBinding CreateBinding()
    {
        var binding = new NetTcpBinding(SecurityMode.Transport);
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        return binding;
    }

    private static string ToRelativeAddress(string path)
    {
        return RelativeAddressPrefix + path;
    }
}
