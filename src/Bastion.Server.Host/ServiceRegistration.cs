using Microsoft.Extensions.DependencyInjection;
using Bastion.Server.Data.Repositories;
using Bastion.Server.Services.Accounts;
using Bastion.Server.Services.Auditing;
using Bastion.Server.Services.Common;
using Bastion.Server.Services.Leaderboard;
using Bastion.Server.Services.Security;
using Bastion.Server.Services.Sessions;

namespace Bastion.Server.Host;

// Every dependency is stateless and each repository call opens its own DbContext, so singletons are safe here;
// the WCF service classes are transient because CoreWCF creates one per call.
public static class ServiceRegistration
{
    public static IServiceCollection AddBastionRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IGameModeRepository, GameModeRepository>();
        services.AddSingleton<ISessionRepository, SessionRepository>();
        services.AddSingleton<IAccessLogRepository, AccessLogRepository>();
        services.AddSingleton<ILeaderboardRepository, LeaderboardRepository>();
        return services;
    }

    public static IServiceCollection AddBastionServices(this IServiceCollection services)
    {
        services.AddSingleton<ICallContext, WcfCallContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ISessionTokenFactory, SessionTokenFactory>();
        services.AddSingleton<IAccessAuditor, AccessAuditor>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ILoginService, LoginService>();
        services.AddSingleton<IRegistrationValidator, RegistrationValidator>();
        services.AddSingleton<IAccountFactory, AccountFactory>();
        services.AddSingleton<IRegistrationService, RegistrationService>();
        services.AddTransient<AccountService>();
        services.AddTransient<LeaderboardService>();
        return services;
    }
}
