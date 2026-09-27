using System.Globalization;
using System.Resources;

namespace Bastion.Client.Localization;

// Hand-maintained typed accessors: the Visual Studio resource generator does not run under dotnet build
// on macOS and Linux. Each property reads one Screen.Element key from Strings.resx.
public static class TextCatalog
{
    private const string ResourceBaseName = "Bastion.Client.Localization.Strings";

    private static readonly ResourceManager _manager = new ResourceManager(
        ResourceBaseName,
        typeof(TextCatalog).Assembly);

    public static string AIDifficultyBoardClassicChip => GetText("AIDifficulty.BoardClassicChip");

    public static string AIDifficultyBoardLabel => GetText("AIDifficulty.BoardLabel");

    public static string AIDifficultyBoardRapidChip => GetText("AIDifficulty.BoardRapidChip");

    public static string AIDifficultyClockChipFormat => GetText("AIDifficulty.ClockChipFormat");

    public static string AIDifficultyClockLabel => GetText("AIDifficulty.ClockLabel");

    public static string AIDifficultyHintToggle => GetText("AIDifficulty.HintToggle");

    public static string AIDifficultyLevelLabel => GetText("AIDifficulty.LevelLabel");

    public static string AIDifficultyNoClockChip => GetText("AIDifficulty.NoClockChip");

    public static string AIDifficultyPlayButton => GetText("AIDifficulty.PlayButton");

    public static string AIDifficultyStrengthFormat => GetText("AIDifficulty.StrengthFormat");

    public static string AIDifficultySubtitle => GetText("AIDifficulty.Subtitle");

    public static string AIDifficultyUndoToggle => GetText("AIDifficulty.UndoToggle");

    public static string AIMatchEndDefeatTitle => GetText("AIMatchEnd.DefeatTitle");

    public static string AIMatchEndExperienceCaption => GetText("AIMatchEnd.ExperienceCaption");

    public static string AIMatchEndMistakeFormat => GetText("AIMatchEnd.MistakeFormat");

    public static string AIMatchEndMistakeOne => GetText("AIMatchEnd.MistakeOne");

    public static string AIMatchEndMistakeThree => GetText("AIMatchEnd.MistakeThree");

    public static string AIMatchEndMistakeTwo => GetText("AIMatchEnd.MistakeTwo");

    public static string AIMatchEndMovesCaption => GetText("AIMatchEnd.MovesCaption");

    public static string AIMatchEndReviewLabel => GetText("AIMatchEnd.ReviewLabel");

    public static string AIMatchEndSubtitle => GetText("AIMatchEnd.Subtitle");

    public static string AIMatchEndVictoryTitle => GetText("AIMatchEnd.VictoryTitle");

    public static string ActiveSessionsChangePasswordBody => GetText("ActiveSessions.ChangePasswordBody");

    public static string ActiveSessionsCloseAllBody => GetText("ActiveSessions.CloseAllBody");

    public static string ActiveSessionsCloseAllButton => GetText("ActiveSessions.CloseAllButton");

    public static string ActiveSessionsCloseBody => GetText("ActiveSessions.CloseBody");

    public static string ActiveSessionsCloseButton => GetText("ActiveSessions.CloseButton");

    public static string ActiveSessionsCurrentBadge => GetText("ActiveSessions.CurrentBadge");

    public static string ActiveSessionsSubtitle => GetText("ActiveSessions.Subtitle");

    public static string AddFriendSearchButton => GetText("AddFriend.SearchButton");

    public static string AddFriendSearchLabel => GetText("AddFriend.SearchLabel");

    public static string AddFriendSearchPlaceholder => GetText("AddFriend.SearchPlaceholder");

    public static string AddFriendSubtitle => GetText("AddFriend.Subtitle");

    public static string AddFriendSuggestionReason => GetText("AddFriend.SuggestionReason");

    public static string AdminPanelGrantButton => GetText("AdminPanel.GrantButton");

    public static string AdminPanelGrantConfirmBody => GetText("AdminPanel.GrantConfirmBody");

    public static string AdminPanelLogsButton => GetText("AdminPanel.LogsButton");

    public static string AdminPanelNicknameLabel => GetText("AdminPanel.NicknameLabel");

    public static string AdminPanelQueueButton => GetText("AdminPanel.QueueButton");

    public static string AdminPanelRegisteredLabel => GetText("AdminPanel.RegisteredLabel");

    public static string AdminPanelStateLabel => GetText("AdminPanel.StateLabel");

    public static string AdminPanelSubtitle => GetText("AdminPanel.Subtitle");

    public static string AdminPanelTypeLabel => GetText("AdminPanel.TypeLabel");

    public static string AiLevelApprentice => GetText("AiLevel.APPRENTICE");

    public static string AiLevelArchitect => GetText("AiLevel.ARCHITECT");

    public static string AiLevelBastion => GetText("AiLevel.BASTION");

    public static string AiLevelBuilder => GetText("AiLevel.BUILDER");

    public static string AppealSendButton => GetText("Appeal.SendButton");

    public static string AppealStillActiveNotice => GetText("Appeal.StillActiveNotice");

    public static string AppealSubtitle => GetText("Appeal.Subtitle");

    public static string AppealTextLabel => GetText("Appeal.TextLabel");

    public static string AppealTextPlaceholder => GetText("Appeal.TextPlaceholder");

    public static string ApplySanctionApplyButton => GetText("ApplySanction.ApplyButton");

    public static string ApplySanctionConfirmBody => GetText("ApplySanction.ConfirmBody");

    public static string ApplySanctionDurationDay => GetText("ApplySanction.DurationDay");

    public static string ApplySanctionDurationLabel => GetText("ApplySanction.DurationLabel");

    public static string ApplySanctionDurationWeek => GetText("ApplySanction.DurationWeek");

    public static string ApplySanctionPlayerLabel => GetText("ApplySanction.PlayerLabel");

    public static string ApplySanctionScopeAccount => GetText("ApplySanction.ScopeAccount");

    public static string ApplySanctionScopeChat => GetText("ApplySanction.ScopeChat");

    public static string ApplySanctionScopeLabel => GetText("ApplySanction.ScopeLabel");

    public static string ApplySanctionSubtitle => GetText("ApplySanction.Subtitle");

    public static string ApplySanctionTypeLabel => GetText("ApplySanction.TypeLabel");

    public static string ApplySanctionTypePermanent => GetText("ApplySanction.TypePermanent");

    public static string ApplySanctionTypeTemporary => GetText("ApplySanction.TypeTemporary");

    public static string BannedAccountAppealButton => GetText("BannedAccount.AppealButton");

    public static string BannedAccountEndLabel => GetText("BannedAccount.EndLabel");

    public static string BannedAccountExitButton => GetText("BannedAccount.ExitButton");

    public static string BannedAccountNotice => GetText("BannedAccount.Notice");

    public static string BannedAccountReasonLabel => GetText("BannedAccount.ReasonLabel");

    public static string BannedAccountSubtitle => GetText("BannedAccount.Subtitle");

    public static string BannedAccountTypeLabel => GetText("BannedAccount.TypeLabel");

    public static string BoxPurchaseConfirmButton => GetText("BoxPurchaseConfirm.Button");

    public static string BoxPurchaseGuaranteeNotice => GetText("BoxPurchaseConfirm.GuaranteeNotice");

    public static string BoxPurchaseSubtitle => GetText("BoxPurchaseConfirm.Subtitle");

    public static string ChangeEmailDoneBody => GetText("ChangeEmail.DoneBody");

    public static string ChangeEmailNewLabel => GetText("ChangeEmail.NewLabel");

    public static string ChangeEmailNewPlaceholder => GetText("ChangeEmail.NewPlaceholder");

    public static string ChangeEmailNotice => GetText("ChangeEmail.Notice");

    public static string ChangeEmailPasswordLabel => GetText("ChangeEmail.PasswordLabel");

    public static string ChangeEmailPasswordPlaceholder => GetText("ChangeEmail.PasswordPlaceholder");

    public static string ChangeEmailSubtitle => GetText("ChangeEmail.Subtitle");

    public static string ChangeNicknameConfirmButton => GetText("ChangeNickname.ConfirmButton");

    public static string ChangeNicknameConfirmDetail => GetText("ChangeNickname.ConfirmDetail");

    public static string ChangeNicknameConfirmFormat => GetText("ChangeNickname.ConfirmFormat");

    public static string ChangeNicknameConfirmTitle => GetText("ChangeNickname.ConfirmTitle");

    public static string ChangeNicknameCurrentLabel => GetText("ChangeNickname.CurrentLabel");

    public static string ChangeNicknameDoneBody => GetText("ChangeNickname.DoneBody");

    public static string ChangeNicknameNewLabel => GetText("ChangeNickname.NewLabel");

    public static string ChangeNicknameNewPlaceholder => GetText("ChangeNickname.NewPlaceholder");

    public static string ChangeNicknameNotice => GetText("ChangeNickname.Notice");

    public static string ChangeNicknameSubtitle => GetText("ChangeNickname.Subtitle");

    public static string ChangePasswordConfirmLabel => GetText("ChangePassword.ConfirmLabel");

    public static string ChangePasswordConfirmPlaceholder => GetText("ChangePassword.ConfirmPlaceholder");

    public static string ChangePasswordCurrentLabel => GetText("ChangePassword.CurrentLabel");

    public static string ChangePasswordCurrentPlaceholder => GetText("ChangePassword.CurrentPlaceholder");

    public static string ChangePasswordDoneBody => GetText("ChangePassword.DoneBody");

    public static string ChangePasswordNewLabel => GetText("ChangePassword.NewLabel");

    public static string ChangePasswordNewPlaceholder => GetText("ChangePassword.NewPlaceholder");

    public static string ChangePasswordSubtitle => GetText("ChangePassword.Subtitle");

    public static string CoinHistoryBalanceCaption => GetText("CoinHistory.BalanceCaption");

    public static string CoinHistorySubtitle => GetText("CoinHistory.Subtitle");

    public static string AvatarPlaceholder => GetText("Common.AvatarPlaceholder");

    public static string CommonBackButton => GetText("Common.BackButton");

    public static string CommonBackLink => GetText("Common.BackLink");

    public static string CommonCancelButton => GetText("Common.CancelButton");

    public static string CommonChangeButton => GetText("Common.ChangeButton");

    public static string CommonContinueButton => GetText("Common.ContinueButton");

    public static string CommonCopyButton => GetText("Common.CopyButton");

    public static string CommonEditButton => GetText("Common.EditButton");

    public static string PasswordPolicyWarning => GetText("Common.PasswordPolicyWarning");

    public static string CommonSaveButton => GetText("Common.SaveButton");

    public static string CommonScreenUnavailable => GetText("Common.ScreenUnavailable");

    public static string CommonServerUnreachable => GetText("Common.ServerUnreachable");

    public static string CommonViewButton => GetText("Common.ViewButton");

    public static string PawnClassic => GetText("CosmeticItem.PAWN_CLASSIC");

    public static string PawnStone => GetText("CosmeticItem.PAWN_STONE");

    public static string TitleBuilder => GetText("CosmeticItem.TITLE_BUILDER");

    public static string TitleRookie => GetText("CosmeticItem.TITLE_ROOKIE");

    public static string TitleStrategist => GetText("CosmeticItem.TITLE_STRATEGIST");

    public static string CustomizeEquipButton => GetText("Customize.EquipButton");

    public static string CustomizePreviewPlaceholder => GetText("Customize.PreviewPlaceholder");

    public static string CustomizeSlotBoard => GetText("Customize.SlotBoard");

    public static string CustomizeSlotEmotes => GetText("Customize.SlotEmotes");

    public static string CustomizeSlotFrame => GetText("Customize.SlotFrame");

    public static string CustomizeSlotPawn => GetText("Customize.SlotPawn");

    public static string CustomizeSlotTitle => GetText("Customize.SlotTitle");

    public static string CustomizeSlotWalls => GetText("Customize.SlotWalls");

    public static string CustomizeSubtitle => GetText("Customize.Subtitle");

    public static string DeleteAccountDeleteButton => GetText("DeleteAccount.DeleteButton");

    public static string DeleteAccountDoneBody => GetText("DeleteAccount.DoneBody");

    public static string DeleteAccountLossFormat => GetText("DeleteAccount.LossFormat");

    public static string DeleteAccountLossTitle => GetText("DeleteAccount.LossTitle");

    public static string DeleteAccountNotice => GetText("DeleteAccount.Notice");

    public static string DeleteAccountPasswordLabel => GetText("DeleteAccount.PasswordLabel");

    public static string DeleteAccountPasswordPlaceholder => GetText("DeleteAccount.PasswordPlaceholder");

    public static string DeleteAccountSubtitle => GetText("DeleteAccount.Subtitle");

    public static string DeleteAccountWord => GetText("DeleteAccount.Word");

    public static string DeleteAccountWordLabelFormat => GetText("DeleteAccount.WordLabelFormat");

    public static string DeleteAccountWrongPassword => GetText("DeleteAccount.WrongPassword");

    public static string DialogAcceptButton => GetText("Dialog.AcceptButton");

    public static string DialogCancelButton => GetText("Dialog.CancelButton");

    public static string DialogConfirmButton => GetText("Dialog.ConfirmButton");

    public static string DialogConfirmTitle => GetText("Dialog.ConfirmTitle");

    public static string DialogErrorTitle => GetText("Dialog.ErrorTitle");

    public static string DialogRetryButton => GetText("Dialog.RetryButton");

    public static string DialogSignInButton => GetText("Dialog.SignInButton");

    public static string DialogSuccessTitle => GetText("Dialog.SuccessTitle");

    public static string DialogWarningTitle => GetText("Dialog.WarningTitle");

    public static string EditProfileIconsButton => GetText("EditProfile.IconsButton");

    public static string EditProfileIconsHint => GetText("EditProfile.IconsHint");

    public static string EditProfileLanguageLabel => GetText("EditProfile.LanguageLabel");

    public static string EditProfileLinkInvalid => GetText("EditProfile.LinkInvalid");

    public static string EditProfileLinkPlaceholder => GetText("EditProfile.LinkPlaceholder");

    public static string EditProfileLinkShortener => GetText("EditProfile.LinkShortener");

    public static string EditProfileLinksHint => GetText("EditProfile.LinksHint");

    public static string EditProfileLinksLabel => GetText("EditProfile.LinksLabel");

    public static string EditProfileMoreIconsChip => GetText("EditProfile.MoreIconsChip");

    public static string EditProfileNicknameHint => GetText("EditProfile.NicknameHint");

    public static string EditProfileNicknameLabel => GetText("EditProfile.NicknameLabel");

    public static string EditProfilePreviewFormat => GetText("EditProfile.PreviewFormat");

    public static string EditProfilePreviewHint => GetText("EditProfile.PreviewHint");

    public static string EditProfileRemoveLinkButton => GetText("EditProfile.RemoveLinkButton");

    public static string EditProfileSavedBody => GetText("EditProfile.SavedBody");

    public static string EditProfileSpectatorsText => GetText("EditProfile.SpectatorsText");

    public static string EditProfileSubtitle => GetText("EditProfile.Subtitle");

    public static string EditProfileTitleLabel => GetText("EditProfile.TitleLabel");

    public static string EditProfileUploadButton => GetText("EditProfile.UploadButton");

    public static string EndReasonAbandonment => GetText("EndReason.ABANDONMENT");

    public static string EndReasonDraw => GetText("EndReason.DRAW");

    public static string EndReasonGoal => GetText("EndReason.GOAL");

    public static string EndReasonResignation => GetText("EndReason.RESIGNATION");

    public static string EndReasonTimeout => GetText("EndReason.TIMEOUT");

    public static string FirstTimeIconLabel => GetText("FirstTime.IconLabel");

    public static string FirstTimeNicknameLabel => GetText("FirstTime.NicknameLabel");

    public static string FirstTimeNotice => GetText("FirstTime.Notice");

    public static string FirstTimePawnLabel => GetText("FirstTime.PawnLabel");

    public static string FirstTimeSkipButton => GetText("FirstTime.SkipButton");

    public static string FirstTimeStartButton => GetText("FirstTime.StartButton");

    public static string FirstTimeSubtitle => GetText("FirstTime.Subtitle");

    public static string ForgotPasswordBackLink => GetText("ForgotPassword.BackLink");

    public static string ForgotPasswordEmailLabel => GetText("ForgotPassword.EmailLabel");

    public static string ForgotPasswordEmailPlaceholder => GetText("ForgotPassword.EmailPlaceholder");

    public static string ForgotPasswordGuestHint => GetText("ForgotPassword.GuestHint");

    public static string ForgotPasswordGuestTitle => GetText("ForgotPassword.GuestTitle");

    public static string ForgotPasswordNotice => GetText("ForgotPassword.Notice");

    public static string ForgotPasswordSendButton => GetText("ForgotPassword.SendButton");

    public static string ForgotPasswordSubtitle => GetText("ForgotPassword.Subtitle");

    public static string FriendsAddButton => GetText("Friends.AddButton");

    public static string FriendsOfflineLabel => GetText("Friends.OfflineLabel");

    public static string FriendsOnlineLabel => GetText("Friends.OnlineLabel");

    public static string FriendsSubtitle => GetText("Friends.Subtitle");

    public static string GameTitle => GetText("Game.Title");

    public static string GameModeClassic => GetText("GameMode.CLASSIC");

    public static string GameModeFourPlayers => GetText("GameMode.FOUR_PLAYERS");

    public static string GameModeQuick => GetText("GameMode.QUICK");

    public static string IconBear => GetText("Icon.BEAR");

    public static string IconCat => GetText("Icon.CAT");

    public static string IconFox => GetText("Icon.FOX");

    public static string IconLighthouse => GetText("Icon.LIGHTHOUSE");

    public static string IconOwl => GetText("Icon.OWL");

    public static string IconPawn => GetText("Icon.PAWN");

    public static string IconWall => GetText("Icon.WALL");

    public static string EnglishLanguageName => GetText("Language.English");

    public static string SpanishMexicoLanguageName => GetText("Language.SpanishMexico");

    public static string LeaderboardEloHeader => GetText("Leaderboard.EloHeader");

    public static string LeaderboardEmpty => GetText("Leaderboard.Empty");

    public static string LeaderboardLoading => GetText("Leaderboard.Loading");

    public static string LeaderboardMatchesHeader => GetText("Leaderboard.MatchesHeader");

    public static string LeaderboardModeLabel => GetText("Leaderboard.ModeLabel");

    public static string LeaderboardNextButton => GetText("Leaderboard.NextButton");

    public static string LeaderboardNotRankedFormat => GetText("Leaderboard.NotRankedFormat");

    public static string LeaderboardPageFormat => GetText("Leaderboard.PageFormat");

    public static string LeaderboardPlayerHeader => GetText("Leaderboard.PlayerHeader");

    public static string LeaderboardPositionHeader => GetText("Leaderboard.PositionHeader");

    public static string LeaderboardPreviousButton => GetText("Leaderboard.PreviousButton");

    public static string LeaderboardSubtitle => GetText("Leaderboard.Subtitle");

    public static string LeaderboardWinsHeader => GetText("Leaderboard.WinsHeader");

    public static string LeaderboardYouLabel => GetText("Leaderboard.YouLabel");

    public static string LessonBoardAndGoal => GetText("Lesson.BOARD_AND_GOAL");

    public static string LessonClock => GetText("Lesson.CLOCK");

    public static string LessonFourPlayers => GetText("Lesson.FOUR_PLAYERS");

    public static string LessonJumpOpponent => GetText("Lesson.JUMP_OPPONENT");

    public static string LessonMovePawn => GetText("Lesson.MOVE_PAWN");

    public static string LessonPlaceWalls => GetText("Lesson.PLACE_WALLS");

    public static string LessonWallsNoTrap => GetText("Lesson.WALLS_NO_TRAP");

    public static string LinkAccountNotice => GetText("LinkAccount.Notice");

    public static string LinkAccountSubtitle => GetText("LinkAccount.Subtitle");

    public static string LinkTypeDiscord => GetText("LinkType.DISCORD");

    public static string LinkTypeOther => GetText("LinkType.OTHER");

    public static string LinkTypeTwitch => GetText("LinkType.TWITCH");

    public static string LinkTypeYoutube => GetText("LinkType.YOUTUBE");

    public static string LoginAccountBanned => GetText("Login.AccountBanned");

    public static string LoginAccountLocked => GetText("Login.AccountLocked");

    public static string LoginAccountPending => GetText("Login.AccountPending");

    public static string LoginAccountSuspended => GetText("Login.AccountSuspended");

    public static string LoginCreateAccountButton => GetText("Login.CreateAccountButton");

    public static string LoginCredentialsRejected => GetText("Login.CredentialsRejected");

    public static string LoginForgotLink => GetText("Login.ForgotLink");

    public static string LoginGuestButton => GetText("Login.GuestButton");

    public static string LoginIdentifierLabel => GetText("Login.IdentifierLabel");

    public static string LoginIdentifierPlaceholder => GetText("Login.IdentifierPlaceholder");

    public static string LoginIdentifierRequired => GetText("Login.IdentifierRequired");

    public static string LoginPasswordLabel => GetText("Login.PasswordLabel");

    public static string LoginPasswordPlaceholder => GetText("Login.PasswordPlaceholder");

    public static string LoginPasswordRequired => GetText("Login.PasswordRequired");

    public static string LoginSignInButton => GetText("Login.SignInButton");

    public static string LoginSignInDetail => GetText("Login.SignInDetail");

    public static string LoginSigningIn => GetText("Login.SigningIn");

    public static string LoginSubtitle => GetText("Login.Subtitle");

    public static string LoginSuccess => GetText("Login.Success");

    public static string LogsAccessOption => GetText("Logs.AccessOption");

    public static string LogsDatePlaceholder => GetText("Logs.DatePlaceholder");

    public static string LogsFromLabel => GetText("Logs.FromLabel");

    public static string LogsModerationOption => GetText("Logs.ModerationOption");

    public static string LogsSearchButton => GetText("Logs.SearchButton");

    public static string LogsSubtitle => GetText("Logs.Subtitle");

    public static string LogsToLabel => GetText("Logs.ToLabel");

    public static string LogsWhichLabel => GetText("Logs.WhichLabel");

    public static string MainMenuCoinsCaption => GetText("MainMenu.CoinsCaption");

    public static string MainMenuCustomizeButton => GetText("MainMenu.CustomizeButton");

    public static string MainMenuEloCaption => GetText("MainMenu.EloCaption");

    public static string MainMenuFindMatchButton => GetText("MainMenu.FindMatchButton");

    public static string MainMenuFindMatchDetail => GetText("MainMenu.FindMatchDetail");

    public static string MainMenuFriendsButton => GetText("MainMenu.FriendsButton");

    public static string MainMenuHistoryButton => GetText("MainMenu.HistoryButton");

    public static string MainMenuHowToPlayButton => GetText("MainMenu.HowToPlayButton");

    public static string MainMenuLevelFormat => GetText("MainMenu.LevelFormat");

    public static string MainMenuModerationButton => GetText("MainMenu.ModerationButton");

    public static string MainMenuPrivateButton => GetText("MainMenu.PrivateButton");

    public static string MainMenuPrivateDetail => GetText("MainMenu.PrivateDetail");

    public static string MainMenuRankingButton => GetText("MainMenu.RankingButton");

    public static string MainMenuSettingsButton => GetText("MainMenu.SettingsButton");

    public static string MainMenuShopButton => GetText("MainMenu.ShopButton");

    public static string MainMenuStreakCaption => GetText("MainMenu.StreakCaption");

    public static string MainMenuSubtitle => GetText("MainMenu.Subtitle");

    public static string MainMenuVersusAiButton => GetText("MainMenu.VersusAiButton");

    public static string MainMenuVersusAiDetail => GetText("MainMenu.VersusAiDetail");

    public static string MainScreenIndexButton => GetText("MainScreen.IndexButton");

    public static string MainScreenIntro => GetText("MainScreen.Intro");

    public static string MainScreenSubtitle => GetText("MainScreen.Subtitle");

    public static string MatchBoardPlaceholder => GetText("Match.BoardPlaceholder");

    public static string MatchChatPlaceholder => GetText("Match.ChatPlaceholder");

    public static string MatchChatSampleOne => GetText("Match.ChatSampleOne");

    public static string MatchChatTitle => GetText("Match.ChatTitle");

    public static string MatchDrawBody => GetText("Match.DrawBody");

    public static string MatchDrawButton => GetText("Match.DrawButton");

    public static string MatchDrawConfirmButton => GetText("Match.DrawConfirmButton");

    public static string MatchMoveChip => GetText("Match.MoveChip");

    public static string MatchResignBody => GetText("Match.ResignBody");

    public static string MatchResignButton => GetText("Match.ResignButton");

    public static string MatchResignConfirmButton => GetText("Match.ResignConfirmButton");

    public static string MatchSubtitle => GetText("Match.Subtitle");

    public static string MatchTurnFormat => GetText("Match.TurnFormat");

    public static string MatchWallChip => GetText("Match.WallChip");

    public static string MatchEndAddOpponentButton => GetText("MatchEnd.AddOpponentButton");

    public static string MatchEndBackMenuButton => GetText("MatchEnd.BackMenuButton");

    public static string MatchEndCoinsCaption => GetText("MatchEnd.CoinsCaption");

    public static string MatchEndDefeatTitle => GetText("MatchEnd.DefeatTitle");

    public static string MatchEndEloCaption => GetText("MatchEnd.EloCaption");

    public static string MatchEndExperienceCaption => GetText("MatchEnd.ExperienceCaption");

    public static string MatchEndFindAnotherButton => GetText("MatchEnd.FindAnotherButton");

    public static string MatchEndVictoryTitle => GetText("MatchEnd.VictoryTitle");

    public static string MatchEndWatchReplayButton => GetText("MatchEnd.WatchReplayButton");

    public static string MatchHistoryLoss => GetText("MatchHistory.Loss");

    public static string MatchHistorySubtitle => GetText("MatchHistory.Subtitle");

    public static string MatchHistoryWin => GetText("MatchHistory.Win");

    public static string MatchmakingBrowseNote => GetText("Matchmaking.BrowseNote");

    public static string MatchmakingElapsedFormat => GetText("Matchmaking.ElapsedFormat");

    public static string MatchmakingSubtitle => GetText("Matchmaking.Subtitle");

    public static string MatchmakingTitle => GetText("Matchmaking.Title");

    public static string ModerationQueueReviewButton => GetText("ModerationQueue.ReviewButton");

    public static string ModerationQueueSubtitle => GetText("ModerationQueue.Subtitle");

    public static string OpponentDisconnectedBackButton => GetText("OpponentDisconnected.BackButton");

    public static string OpponentDisconnectedClaimButton => GetText("OpponentDisconnected.ClaimButton");

    public static string OpponentDisconnectedElapsedFormat => GetText("OpponentDisconnected.ElapsedFormat");

    public static string OpponentDisconnectedHint => GetText("OpponentDisconnected.Hint");

    public static string OpponentDisconnectedTitle => GetText("OpponentDisconnected.Title");

    public static string PendingVerificationAcceptButton => GetText("PendingVerification.AcceptButton");

    public static string PendingVerificationNoticeFormat => GetText("PendingVerification.NoticeFormat");

    public static string PendingVerificationResendButton => GetText("PendingVerification.ResendButton");

    public static string PendingVerificationResentBody => GetText("PendingVerification.ResentBody");

    public static string PendingVerificationSubtitle => GetText("PendingVerification.Subtitle");

    public static string PlayerCardAddFriendButton => GetText("PlayerCard.AddFriendButton");

    public static string PlayerCardHeadToHeadCaption => GetText("PlayerCard.HeadToHeadCaption");

    public static string PlayerCardReportButton => GetText("PlayerCard.ReportButton");

    public static string PlayerCardSubtitle => GetText("PlayerCard.Subtitle");

    public static string PlayerCardWatchButton => GetText("PlayerCard.WatchButton");

    public static string PrivateMatchCodeLabel => GetText("PrivateMatch.CodeLabel");

    public static string PrivateMatchCodePlaceholder => GetText("PrivateMatch.CodePlaceholder");

    public static string PrivateMatchCodeRequired => GetText("PrivateMatch.CodeRequired");

    public static string PrivateMatchCreateButton => GetText("PrivateMatch.CreateButton");

    public static string PrivateMatchCreateHint => GetText("PrivateMatch.CreateHint");

    public static string PrivateMatchCreateTitle => GetText("PrivateMatch.CreateTitle");

    public static string PrivateMatchJoinButton => GetText("PrivateMatch.JoinButton");

    public static string PrivateMatchJoinTitle => GetText("PrivateMatch.JoinTitle");

    public static string PrivateMatchSubtitle => GetText("PrivateMatch.Subtitle");

    public static string ProfileAddLinkChip => GetText("Profile.AddLinkChip");

    public static string ProfileAvatarPlaceholder => GetText("Profile.AvatarPlaceholder");

    public static string ProfileAverageLengthFormat => GetText("Profile.AverageLengthFormat");

    public static string ProfileByModeTitle => GetText("Profile.ByModeTitle");

    public static string ProfileHeaderFormat => GetText("Profile.HeaderFormat");

    public static string ProfileLinksTitle => GetText("Profile.LinksTitle");

    public static string ProfileMatchesTile => GetText("Profile.MatchesTile");

    public static string ProfileModeStatFormat => GetText("Profile.ModeStatFormat");

    public static string ProfileNoTitle => GetText("Profile.NoTitle");

    public static string ProfileSeeAllLink => GetText("Profile.SeeAllLink");

    public static string ProfileStreakTile => GetText("Profile.StreakTile");

    public static string ProfileSubtitle => GetText("Profile.Subtitle");

    public static string ProfileTitlesTitle => GetText("Profile.TitlesTitle");

    public static string ProfileTopEloTile => GetText("Profile.TopEloTile");

    public static string ProfileWinsTile => GetText("Profile.WinsTile");

    public static string PurchaseConfirmBalanceLabel => GetText("PurchaseConfirm.BalanceLabel");

    public static string PurchaseConfirmButton => GetText("PurchaseConfirm.Button");

    public static string PurchaseConfirmItemLabel => GetText("PurchaseConfirm.ItemLabel");

    public static string PurchaseConfirmPriceLabel => GetText("PurchaseConfirm.PriceLabel");

    public static string PurchaseConfirmRemainingLabel => GetText("PurchaseConfirm.RemainingLabel");

    public static string PurchaseConfirmSubtitle => GetText("PurchaseConfirm.Subtitle");

    public static string RegisterBirthDateFuture => GetText("Register.BirthDateFuture");

    public static string RegisterBirthDateInvalid => GetText("Register.BirthDateInvalid");

    public static string RegisterBirthDateLabel => GetText("Register.BirthDateLabel");

    public static string RegisterCancelButton => GetText("Register.CancelButton");

    public static string RegisterCheckTheForm => GetText("Register.CheckTheForm");

    public static string RegisterConfirmationLabel => GetText("Register.ConfirmationLabel");

    public static string RegisterConfirmationMismatch => GetText("Register.ConfirmationMismatch");

    public static string RegisterConfirmationPlaceholder => GetText("Register.ConfirmationPlaceholder");

    public static string RegisterCreateButton => GetText("Register.CreateButton");

    public static string RegisterCreateButtonDetail => GetText("Register.CreateButtonDetail");

    public static string RegisterCreating => GetText("Register.Creating");

    public static string RegisterDayPlaceholder => GetText("Register.DayPlaceholder");

    public static string RegisterDiscardBody => GetText("Register.DiscardBody");

    public static string RegisterDiscardButton => GetText("Register.DiscardButton");

    public static string RegisterEmailInvalid => GetText("Register.EmailInvalid");

    public static string RegisterEmailLabel => GetText("Register.EmailLabel");

    public static string RegisterEmailPlaceholder => GetText("Register.EmailPlaceholder");

    public static string RegisterEmailTaken => GetText("Register.EmailTaken");

    public static string RegisterKeepEditingButton => GetText("Register.KeepEditingButton");

    public static string RegisterMonthPlaceholder => GetText("Register.MonthPlaceholder");

    public static string RegisterNicknameInvalid => GetText("Register.NicknameInvalid");

    public static string RegisterNicknameLabel => GetText("Register.NicknameLabel");

    public static string RegisterNicknameLength => GetText("Register.NicknameLength");

    public static string RegisterNicknamePlaceholder => GetText("Register.NicknamePlaceholder");

    public static string RegisterNicknameTaken => GetText("Register.NicknameTaken");

    public static string RegisterPasswordLabel => GetText("Register.PasswordLabel");

    public static string RegisterPasswordPlaceholder => GetText("Register.PasswordPlaceholder");

    public static string RegisterPasswordPolicy => GetText("Register.PasswordPolicy");

    public static string RegisterPasswordTooShort => GetText("Register.PasswordTooShort");

    public static string RegisterRejected => GetText("Register.Rejected");

    public static string RegisterServerUnreachable => GetText("Register.ServerUnreachable");

    public static string RegisterSubtitle => GetText("Register.Subtitle");

    public static string RegisterTermsLinkText => GetText("Register.TermsLinkText");

    public static string RegisterTermsRequired => GetText("Register.TermsRequired");

    public static string RegisterTermsText => GetText("Register.TermsText");

    public static string RegisterUnderage => GetText("Register.Underage");

    public static string RegisterYearPlaceholder => GetText("Register.YearPlaceholder");

    public static string RegistrationSuccessDetail => GetText("RegistrationSuccess.Detail");

    public static string RegistrationSuccessEmailLabel => GetText("RegistrationSuccess.EmailLabel");

    public static string RegistrationSuccessNotice => GetText("RegistrationSuccess.Notice");

    public static string RegistrationSuccessSignInButton => GetText("RegistrationSuccess.SignInButton");

    public static string RegistrationSuccessSubtitle => GetText("RegistrationSuccess.Subtitle");

    public static string ReplayExitButton => GetText("Replay.ExitButton");

    public static string ReplayMoveFormat => GetText("Replay.MoveFormat");

    public static string ReplayNextButton => GetText("Replay.NextButton");

    public static string ReplayNotationTitle => GetText("Replay.NotationTitle");

    public static string ReplayPauseButton => GetText("Replay.PauseButton");

    public static string ReplayPlayButton => GetText("Replay.PlayButton");

    public static string ReplayPreviousButton => GetText("Replay.PreviousButton");

    public static string ReplayResultFormat => GetText("Replay.ResultFormat");

    public static string ReplayTitle => GetText("Replay.Title");

    public static string ReportDescriptionLabel => GetText("Report.DescriptionLabel");

    public static string ReportDescriptionPlaceholder => GetText("Report.DescriptionPlaceholder");

    public static string ReportEvidenceNotice => GetText("Report.EvidenceNotice");

    public static string ReportReasonAbuse => GetText("Report.ReasonAbuse");

    public static string ReportReasonCheating => GetText("Report.ReasonCheating");

    public static string ReportReasonLeaving => GetText("Report.ReasonLeaving");

    public static string ReportReasonName => GetText("Report.ReasonName");

    public static string ReportReasonOther => GetText("Report.ReasonOther");

    public static string ReportSendButton => GetText("Report.SendButton");

    public static string ReportSubtitle => GetText("Report.Subtitle");

    public static string ReportReviewDismissButton => GetText("ReportReview.DismissButton");

    public static string ReportReviewReasonLabel => GetText("ReportReview.ReasonLabel");

    public static string ReportReviewSanctionButton => GetText("ReportReview.SanctionButton");

    public static string ReportReviewSubtitle => GetText("ReportReview.Subtitle");

    public static string ResetPasswordCodeInvalid => GetText("ResetPassword.CodeInvalid");

    public static string ResetPasswordCodeLabel => GetText("ResetPassword.CodeLabel");

    public static string ResetPasswordCodePlaceholder => GetText("ResetPassword.CodePlaceholder");

    public static string ResetPasswordConfirmLabel => GetText("ResetPassword.ConfirmLabel");

    public static string ResetPasswordConfirmPlaceholder => GetText("ResetPassword.ConfirmPlaceholder");

    public static string ResetPasswordDoneBody => GetText("ResetPassword.DoneBody");

    public static string ResetPasswordNewLabel => GetText("ResetPassword.NewLabel");

    public static string ResetPasswordNewPlaceholder => GetText("ResetPassword.NewPlaceholder");

    public static string ResetPasswordResendButton => GetText("ResetPassword.ResendButton");

    public static string ResetPasswordResendInFormat => GetText("ResetPassword.ResendInFormat");

    public static string ResetPasswordSaveButton => GetText("ResetPassword.SaveButton");

    public static string ResetPasswordSentFormat => GetText("ResetPassword.SentFormat");

    public static string ResetPasswordSentTitle => GetText("ResetPassword.SentTitle");

    public static string ResetPasswordSessionsNote => GetText("ResetPassword.SessionsNote");

    public static string ResetPasswordSubtitle => GetText("ResetPassword.Subtitle");

    public static string SecondFactorCodeLabel => GetText("SecondFactor.CodeLabel");

    public static string SecondFactorCodePlaceholder => GetText("SecondFactor.CodePlaceholder");

    public static string SecondFactorExpiresFormat => GetText("SecondFactor.ExpiresFormat");

    public static string SecondFactorHint => GetText("SecondFactor.Hint");

    public static string SecondFactorResendButton => GetText("SecondFactor.ResendButton");

    public static string SecondFactorResentBody => GetText("SecondFactor.ResentBody");

    public static string SecondFactorSubtitle => GetText("SecondFactor.Subtitle");

    public static string SecondFactorVerifyButton => GetText("SecondFactor.VerifyButton");

    public static string SelectModeClockFormat => GetText("SelectMode.ClockFormat");

    public static string SelectModeClockLabel => GetText("SelectMode.ClockLabel");

    public static string SelectModeModeLabel => GetText("SelectMode.ModeLabel");

    public static string SelectModeSearchButton => GetText("SelectMode.SearchButton");

    public static string SelectModeSubtitle => GetText("SelectMode.Subtitle");

    public static string SettingsAccessibilityEntry => GetText("Settings.AccessibilityEntry");

    public static string SettingsAccountEntry => GetText("Settings.AccountEntry");

    public static string SettingsAudioEntry => GetText("Settings.AudioEntry");

    public static string SettingsBoardEntry => GetText("Settings.BoardEntry");

    public static string SettingsChatNote => GetText("Settings.ChatNote");

    public static string SettingsCurrentTag => GetText("Settings.CurrentTag");

    public static string SettingsDeleteHint => GetText("Settings.DeleteHint");

    public static string SettingsDeleteTitle => GetText("Settings.DeleteTitle");

    public static string SettingsEmailRow => GetText("Settings.EmailRow");

    public static string SettingsEmailVerified => GetText("Settings.EmailVerified");

    public static string SettingsFriendCodeRow => GetText("Settings.FriendCodeRow");

    public static string SettingsHeading => GetText("Settings.Heading");

    public static string SettingsInterfaceLanguageLabel => GetText("Settings.InterfaceLanguageLabel");

    public static string SettingsLanguageEntry => GetText("Settings.LanguageEntry");

    public static string SettingsPasswordChangedFormat => GetText("Settings.PasswordChangedFormat");

    public static string SettingsPasswordRow => GetText("Settings.PasswordRow");

    public static string SettingsSessionsCountFormat => GetText("Settings.SessionsCountFormat");

    public static string SettingsSessionsRow => GetText("Settings.SessionsRow");

    public static string SettingsSignOutBody => GetText("Settings.SignOutBody");

    public static string SettingsSignOutButton => GetText("Settings.SignOutButton");

    public static string ShopBoxButton => GetText("Shop.BoxButton");

    public static string ShopBuyButton => GetText("Shop.BuyButton");

    public static string ShopSubtitle => GetText("Shop.Subtitle");

    public static string SpectatorChatPlaceholder => GetText("Spectator.ChatPlaceholder");

    public static string SpectatorChatTitle => GetText("Spectator.ChatTitle");

    public static string SpectatorDelayFormat => GetText("Spectator.DelayFormat");

    public static string SpectatorLeaveButton => GetText("Spectator.LeaveButton");

    public static string SpectatorLiveTag => GetText("Spectator.LiveTag");

    public static string SpectatorMovesTitle => GetText("Spectator.MovesTitle");

    public static string SpectatorSubtitle => GetText("Spectator.Subtitle");

    public static string SpectatorViewersFormat => GetText("Spectator.ViewersFormat");

    public static string TutorialIndexDoneTag => GetText("TutorialIndex.DoneTag");

    public static string TutorialIndexExitButton => GetText("TutorialIndex.ExitButton");

    public static string TutorialIndexInProgressTag => GetText("TutorialIndex.InProgressTag");

    public static string TutorialIndexPracticeButton => GetText("TutorialIndex.PracticeButton");

    public static string TutorialIndexProgressFormat => GetText("TutorialIndex.ProgressFormat");

    public static string TutorialIndexSubtitle => GetText("TutorialIndex.Subtitle");

    public static string VersusScreenClockFormat => GetText("VersusScreen.ClockFormat");

    public static string VersusScreenEloFormat => GetText("VersusScreen.EloFormat");

    public static string VersusScreenSettingsFormat => GetText("VersusScreen.SettingsFormat");

    public static string VersusScreenStartButton => GetText("VersusScreen.StartButton");

    public static string VersusScreenSubtitle => GetText("VersusScreen.Subtitle");

    public static string VersusScreenVersusLabel => GetText("VersusScreen.VersusLabel");

    public static string WaitingRoomChatPlaceholder => GetText("WaitingRoom.ChatPlaceholder");

    public static string WaitingRoomCodeLabel => GetText("WaitingRoom.CodeLabel");

    public static string WaitingRoomFreeSlot => GetText("WaitingRoom.FreeSlot");

    public static string WaitingRoomHostNote => GetText("WaitingRoom.HostNote");

    public static string WaitingRoomHostTag => GetText("WaitingRoom.HostTag");

    public static string WaitingRoomLeaveBody => GetText("WaitingRoom.LeaveBody");

    public static string WaitingRoomLeaveButton => GetText("WaitingRoom.LeaveButton");

    public static string WaitingRoomPlayersLabel => GetText("WaitingRoom.PlayersLabel");

    public static string WaitingRoomReadyButton => GetText("WaitingRoom.ReadyButton");

    public static string WaitingRoomStartButton => GetText("WaitingRoom.StartButton");

    public static string WaitingRoomSubtitle => GetText("WaitingRoom.Subtitle");

    public static string WaitingRoomYouTag => GetText("WaitingRoom.YouTag");

    public static string WindowTitle => GetText("Window.Title");

    public static string ShopEmpty => GetText("Shop.Empty");

    public static string CoinHistoryEmpty => GetText("CoinHistory.Empty");

    public static string ModerationQueueEmpty => GetText("ModerationQueue.Empty");

    public static string LogsEmpty => GetText("Logs.Empty");

    public static string PendingVerificationNotice => GetText("PendingVerification.Notice");

    public static string ActiveSessionsEmpty => GetText("ActiveSessions.Empty");

    public static string FriendsEmpty => GetText("Friends.Empty");

    public static string PlayerCardEmpty => GetText("PlayerCard.Empty");

    public static string MatchHistoryEmpty => GetText("MatchHistory.Empty");

    // A missing key shows the key itself so the gap is visible on screen and in the catalog tests.
    public static string GetText(string key)
    {
        return _manager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
