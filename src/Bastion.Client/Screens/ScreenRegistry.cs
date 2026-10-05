using System;
using System.Collections.Generic;
using Bastion.Client.Screens.AIDifficulty;
using Bastion.Client.Screens.AIMatchEnd;
using Bastion.Client.Screens.ActiveSessions;
using Bastion.Client.Screens.AddFriend;
using Bastion.Client.Screens.AdminPanel;
using Bastion.Client.Screens.Appeal;
using Bastion.Client.Screens.ApplySanction;
using Bastion.Client.Screens.BannedAccount;
using Bastion.Client.Screens.BoxPurchaseConfirm;
using Bastion.Client.Screens.ChangeEmail;
using Bastion.Client.Screens.ChangeNickname;
using Bastion.Client.Screens.ChangePassword;
using Bastion.Client.Screens.CoinHistory;
using Bastion.Client.Screens.Customize;
using Bastion.Client.Screens.DeleteAccount;
using Bastion.Client.Screens.EditProfile;
using Bastion.Client.Screens.FirstTime;
using Bastion.Client.Screens.ForgotPassword;
using Bastion.Client.Screens.Friends;
using Bastion.Client.Screens.Leaderboard;
using Bastion.Client.Screens.LinkAccount;
using Bastion.Client.Screens.Login;
using Bastion.Client.Screens.Logs;
using Bastion.Client.Screens.MainMenu;
using Bastion.Client.Screens.Match;
using Bastion.Client.Screens.MatchEnd;
using Bastion.Client.Screens.MatchHistory;
using Bastion.Client.Screens.Matchmaking;
using Bastion.Client.Screens.ModerationQueue;
using Bastion.Client.Screens.OpponentDisconnected;
using Bastion.Client.Screens.PendingVerification;
using Bastion.Client.Screens.PlayerCard;
using Bastion.Client.Screens.PrivateMatch;
using Bastion.Client.Screens.Profile;
using Bastion.Client.Screens.PurchaseConfirm;
using Bastion.Client.Screens.Register;
using Bastion.Client.Screens.RegistrationSuccess;
using Bastion.Client.Screens.Replay;
using Bastion.Client.Screens.Report;
using Bastion.Client.Screens.ReportReview;
using Bastion.Client.Screens.ResetPassword;
using Bastion.Client.Screens.SecondFactor;
using Bastion.Client.Screens.SelectMode;
using Bastion.Client.Screens.Settings;
using Bastion.Client.Screens.Shop;
using Bastion.Client.Screens.Spectator;
using Bastion.Client.Screens.Title;
using Bastion.Client.Screens.TutorialIndex;
using Bastion.Client.Screens.Versus;
using Bastion.Client.Screens.WaitingRoom;

namespace Bastion.Client.Screens;

public static class ScreenRegistry
{
    private static readonly Dictionary<ScreenId, Func<ScreenContext, IScreen>> _factories = new()
    {
        [ScreenId.MainScreen] = context => new TitleScreen(context.Navigator),
        [ScreenId.Login] = context => CreateLogin(context),
        [ScreenId.Register] =
            context => new RegisterScreen(context.Navigator, new RegisterController(context.Services.Accounts)),
        [ScreenId.RegistrationSuccess] =
            context => new RegistrationSuccessScreen(context.Navigator, context.Argument ?? string.Empty),
        [ScreenId.MainMenu] = context => new MainMenuScreen(context.Navigator, context.Services.Session),
        [ScreenId.Leaderboard] = context => CreateLeaderboard(context),
        [ScreenId.Shop] = context => new ShopScreen(context.Navigator),
        [ScreenId.PurchaseConfirm] = context => new PurchaseConfirmScreen(context.Navigator),
        [ScreenId.BoxPurchaseConfirm] = context => new BoxPurchaseConfirmScreen(context.Navigator),
        [ScreenId.CoinHistory] = context => new CoinHistoryScreen(context.Navigator),
        [ScreenId.Report] = context => new ReportScreen(context.Navigator),
        [ScreenId.ReportReview] = context => new ReportReviewScreen(context.Navigator),
        [ScreenId.ModerationQueue] = context => new ModerationQueueScreen(context.Navigator),
        [ScreenId.ApplySanction] = context => new ApplySanctionScreen(context.Navigator),
        [ScreenId.Appeal] = context => new AppealScreen(context.Navigator),
        [ScreenId.BannedAccount] = context => new BannedAccountScreen(context.Navigator),
        [ScreenId.AdminPanel] = context => new AdminPanelScreen(context.Navigator),
        [ScreenId.Logs] = context => new LogsScreen(context.Navigator),
        [ScreenId.ChangePassword] = context => new ChangePasswordScreen(context.Navigator),
        [ScreenId.ChangeEmail] = context => new ChangeEmailScreen(context.Navigator),
        [ScreenId.ChangeNickname] = context => new ChangeNicknameScreen(context.Navigator),
        [ScreenId.ForgotPassword] = context => new ForgotPasswordScreen(context.Navigator),
        [ScreenId.ResetPassword] =
            context => new ResetPasswordScreen(context.Navigator, context.Argument ?? string.Empty),
        [ScreenId.PendingVerification] =
            context => new PendingVerificationScreen(context.Navigator, context.Argument ?? string.Empty),
        [ScreenId.SecondFactor] = context => new SecondFactorScreen(context.Navigator),
        [ScreenId.LinkAccount] = context => new LinkAccountScreen(context.Navigator),
        [ScreenId.FirstTime] = context => new FirstTimeScreen(context.Navigator),
        [ScreenId.DeleteAccount] = context => new DeleteAccountScreen(context.Navigator),
        [ScreenId.ActiveSessions] = context => new ActiveSessionsScreen(context.Navigator),
        [ScreenId.AccountSettings] = context => new SettingsScreen(context.Navigator, false),
        [ScreenId.SettingsLanguage] = context => new SettingsScreen(context.Navigator, true),
        [ScreenId.Profile] = context => new ProfileScreen(context.Navigator),
        [ScreenId.EditProfile] = context => new EditProfileScreen(context.Navigator),
        [ScreenId.PlayerCard] = context => new PlayerCardScreen(context.Navigator),
        [ScreenId.Friends] = context => new FriendsScreen(context.Navigator),
        [ScreenId.AddFriend] = context => new AddFriendScreen(context.Navigator),
        [ScreenId.Customize] = context => new CustomizeScreen(context.Navigator),
        [ScreenId.SelectMode] = context => new SelectModeScreen(context.Navigator),
        [ScreenId.PrivateMatch] = context => new PrivateMatchScreen(context.Navigator),
        [ScreenId.WaitingRoom] =
            context => new WaitingRoomScreen(context.Navigator, context.Argument ?? ScreenArgument.HostRole),
        [ScreenId.Matchmaking] = context => new MatchmakingScreen(context.Navigator),
        [ScreenId.VersusScreen] = context => new VersusScreen(context.Navigator),
        [ScreenId.Match] = context => new MatchScreen(context.Navigator, context.Argument),
        [ScreenId.MatchEnd] =
            context => new MatchEndScreen(context.Navigator, context.Argument ?? ScreenArgument.DefeatOutcome),
        [ScreenId.OpponentDisconnected] = context => new OpponentDisconnectedScreen(context.Navigator),
        [ScreenId.MatchHistory] = context => new MatchHistoryScreen(context.Navigator),
        [ScreenId.Replay] = context => new ReplayScreen(context.Navigator),
        [ScreenId.Spectator] = context => new SpectatorScreen(context.Navigator),
        [ScreenId.AIDifficulty] = context => new AIDifficultyScreen(context.Navigator),
        [ScreenId.AIMatchEnd] = context => new AIMatchEndScreen(context.Navigator),
        [ScreenId.TutorialIndex] = context => new TutorialIndexScreen(context.Navigator),
    };

    public static IReadOnlyCollection<ScreenId> RegisteredScreens => _factories.Keys;

    public static bool Contains(ScreenId screen)
    {
        return _factories.ContainsKey(screen);
    }

    public static IScreen Create(ScreenId screen, ScreenContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!_factories.TryGetValue(screen, out Func<ScreenContext, IScreen>? factory))
        {
            throw new InvalidOperationException($"The screen is not registered. Screen={screen}");
        }

        return factory(context);
    }

    private static IScreen CreateLogin(ScreenContext context)
    {
        var controller = new LoginController(context.Services.Accounts, context.Services.Session);
        return new LoginScreen(context.Navigator, controller);
    }

    private static IScreen CreateLeaderboard(ScreenContext context)
    {
        var controller = new LeaderboardController(context.Services.Leaderboard, context.Services.Session);
        return new LeaderboardScreen(context.Navigator, controller);
    }
}
