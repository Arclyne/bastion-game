namespace Bastion.Client.Screens;

// The screens the navigator can reach by name, so a screen asks for a
// destination without depending on the class that implements it.
public enum ScreenId
{
    Login,
    Register,
    ForgotPassword,
    ResetPassword,
    AccountSettings,
    ChangePassword,
    ChangeEmail,
    ChangeNickname,
    DeleteAccount,
    ActiveSessions,
    RegistrationSuccess,
    Profile,
    EditProfile,
    PlayerCard,
    Friends,
    AddFriend,
    MatchHistory,
    CoinHistory,
    Leaderboard,
    Report,
    ModerationQueue,
    ReportReview,
    ApplySanction,
    Appeal,
    AdminPanel,
    Logs,
    Shop,
    PurchaseConfirm,
    BoxPurchaseConfirm,
    Customize,
    SettingsLanguage,

    // Not reachable yet from Login or from a session: the server responses that
    // trigger them (an unverified account, a sanction, a second factor) do not
    // exist yet.
    MainScreen,
    PendingVerification,
    SecondFactor,
    BannedAccount,
    FirstTime,
    LinkAccount,

    // The home hub and the match flow it opens.
    MainMenu,
    SelectMode,
    Matchmaking,
    VersusScreen,
    Match,
    MatchEnd,
    OpponentDisconnected,
    PrivateMatch,
    WaitingRoom,
    AIDifficulty,
    AIMatchEnd,
    Spectator,
    Replay,
    TutorialIndex,

    // The board on its own, with no interface over it. It is where the board is
    // built and looked at; the client opens it directly with --board.
    BoardScene
}
