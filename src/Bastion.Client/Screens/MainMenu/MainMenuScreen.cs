using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Session;

namespace Bastion.Client.Screens.MainMenu;

// CU-01 main flow step 19. The home hub once a session is open: the player's numbers up top, the three ways to
// start a match, and the rest of the game one tap away. Prototype 3.4.
public sealed class MainMenuScreen : FormScreen
{
    private const int HeaderHeight = 24;
    private const int SectionGap = 20;
    private const int TileHeight = 68;
    private const int TileGap = 14;
    private const int TileCount = 3;
    private const int ActionHeight = 68;
    private const int ActionGapRow = 14;
    private const int ActionCount = 3;
    private const int SmallButtonGap = 12;
    private const int SmallButtonCount = 5;
    private const int FooterGap = 12;
    private const int FooterCount = 3;

    private const int HeaderGap = 8;
    private const int LevelLineOffset = 4;
    private const int EloTileIndex = 0;
    private const int StreakTileIndex = 1;
    private const int CoinsTileIndex = 2;
    private const int FindMatchIndex = 0;
    private const int PrivateMatchIndex = 1;
    private const int VersusAiIndex = 2;
    private const int FriendsIndex = 0;
    private const int CustomizeIndex = 1;
    private const int ShopIndex = 2;
    private const int LeaderboardIndex = 3;
    private const int HistoryIndex = 4;
    private const int TilesTop = HeaderHeight + HeaderGap;
    private const int ActionsTop = TilesTop + TileHeight + SectionGap;
    private const int SmallTop = ActionsTop + ActionHeight + SectionGap;
    private const int FooterTop = SmallTop + Theme.SecondaryButtonHeight + SectionGap;
    private const int CardHeight = Theme.CardPadding + FooterTop + Theme.SecondaryButtonHeight + Theme.CardPadding;

    private readonly SessionContext _session;
    private readonly TextLine _header;
    private readonly TextLine _levelLine;
    private readonly StatTile _eloTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _coinsTile;
    private readonly Button _findMatchButton;
    private readonly Button _privateMatchButton;
    private readonly Button _versusAiButton;
    private readonly Button _friendsButton;
    private readonly Button _customizeButton;
    private readonly Button _shopButton;
    private readonly Button _leaderboardButton;
    private readonly Button _historyButton;
    private readonly Button _howToPlayButton;
    private readonly Button _settingsButton;
    private readonly Button _moderationButton;

    public MainMenuScreen(INavigator navigator, SessionContext session)
        : base(navigator, WideCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(session);

        _session = session;
        int top = Card.Y + Theme.CardPadding;

        _header = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HeaderHeight)
        };
        _levelLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsRightAligned = true,
            Bounds = new Rectangle(ContentX, top + LevelLineOffset, ContentWidth, HeaderHeight)
        };

        int tileWidth = (ContentWidth - (TileGap * (TileCount - 1))) / TileCount;
        _eloTile = CreateTile(EloTileIndex, tileWidth, top + TilesTop);
        _streakTile = CreateTile(StreakTileIndex, tileWidth, top + TilesTop);
        _coinsTile = CreateTile(CoinsTileIndex, tileWidth, top + TilesTop);

        int actionWidth = (ContentWidth - (ActionGapRow * (ActionCount - 1))) / ActionCount;
        _findMatchButton = CreateActionButton(FindMatchIndex, actionWidth, top + ActionsTop);
        _privateMatchButton = CreateActionButton(PrivateMatchIndex, actionWidth, top + ActionsTop);
        _versusAiButton = CreateActionButton(VersusAiIndex, actionWidth, top + ActionsTop);
        _findMatchButton.Clicked += OnFindMatchClicked;
        _privateMatchButton.Clicked += OnPrivateMatchClicked;
        _versusAiButton.Clicked += OnVersusAiClicked;

        int smallWidth = (ContentWidth - (SmallButtonGap * (SmallButtonCount - 1))) / SmallButtonCount;
        _friendsButton = CreateSmallButton(FriendsIndex, smallWidth, top + SmallTop);
        _customizeButton = CreateSmallButton(CustomizeIndex, smallWidth, top + SmallTop);
        _shopButton = CreateSmallButton(ShopIndex, smallWidth, top + SmallTop);
        _leaderboardButton = CreateSmallButton(LeaderboardIndex, smallWidth, top + SmallTop);
        _historyButton = CreateSmallButton(HistoryIndex, smallWidth, top + SmallTop);
        _friendsButton.Clicked += OnFriendsClicked;
        _customizeButton.Clicked += OnCustomizeClicked;
        _shopButton.Clicked += OnShopClicked;
        _leaderboardButton.Clicked += OnLeaderboardClicked;
        _historyButton.Clicked += OnHistoryClicked;

        int footerWidth = (ContentWidth - (FooterGap * (FooterCount - 1))) / FooterCount;
        _howToPlayButton = CreateOutlineButton(
            new Rectangle(ContentX, top + FooterTop, footerWidth, Theme.SecondaryButtonHeight));
        int moderationX = ContentX + footerWidth + FooterGap;
        _moderationButton = CreateOutlineButton(
            new Rectangle(moderationX, top + FooterTop, footerWidth, Theme.SecondaryButtonHeight));
        int settingsX = moderationX + footerWidth + FooterGap;
        _settingsButton = CreateOutlineButton(
            new Rectangle(settingsX, top + FooterTop, footerWidth, Theme.SecondaryButtonHeight));
        _howToPlayButton.Clicked += OnHowToPlayClicked;
        _moderationButton.Clicked += OnModerationClicked;
        _settingsButton.Clicked += OnSettingsClicked;

        Register(_header);
        Register(_levelLine);
        Register(_eloTile);
        Register(_streakTile);
        Register(_coinsTile);
        Register(_findMatchButton);
        Register(_privateMatchButton);
        Register(_versusAiButton);
        Register(_friendsButton);
        Register(_customizeButton);
        Register(_shopButton);
        Register(_leaderboardButton);
        Register(_historyButton);
        Register(_howToPlayButton);
        Register(_moderationButton);
        Register(_settingsButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MainMenuSubtitle;
    }

    private void OnFindMatchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.SelectMode);
    }

    private void OnPrivateMatchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.PrivateMatch);
    }

    private void OnVersusAiClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AIDifficulty);
    }

    private void OnFriendsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Friends);
    }

    private void OnCustomizeClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Customize);
    }

    private void OnShopClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Shop);
    }

    private void OnLeaderboardClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Leaderboard);
    }

    private void OnHistoryClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MatchHistory);
    }

    private void OnHowToPlayClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.TutorialIndex);
    }

    private void OnSettingsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AccountSettings);
    }

    // Who may open it is the server's call; the entry is always drawn.
    private void OnModerationClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AdminPanel);
    }

    // The level, elo, streak and coins arrive with the profile service (CU-15); until then the tiles show only
    // their captions instead of made-up numbers.
    protected override void ApplyTexts()
    {
        _header.Text = _session.Nickname;
        _levelLine.Text = string.Empty;

        _eloTile.Value = string.Empty;
        _eloTile.Caption = TextCatalog.MainMenuEloCaption;
        _streakTile.Value = string.Empty;
        _streakTile.Caption = TextCatalog.MainMenuStreakCaption;
        _coinsTile.Value = string.Empty;
        _coinsTile.Caption = TextCatalog.MainMenuCoinsCaption;

        _findMatchButton.Title = TextCatalog.MainMenuFindMatchButton;
        _findMatchButton.Subtitle = TextCatalog.MainMenuFindMatchDetail;
        _privateMatchButton.Title = TextCatalog.MainMenuPrivateButton;
        _privateMatchButton.Subtitle = TextCatalog.MainMenuPrivateDetail;
        _versusAiButton.Title = TextCatalog.MainMenuVersusAiButton;
        _versusAiButton.Subtitle = TextCatalog.MainMenuVersusAiDetail;

        _friendsButton.Title = TextCatalog.MainMenuFriendsButton;
        _customizeButton.Title = TextCatalog.MainMenuCustomizeButton;
        _shopButton.Title = TextCatalog.MainMenuShopButton;
        _leaderboardButton.Title = TextCatalog.MainMenuRankingButton;
        _historyButton.Title = TextCatalog.MainMenuHistoryButton;

        _howToPlayButton.Title = TextCatalog.MainMenuHowToPlayButton;
        _moderationButton.Title = TextCatalog.MainMenuModerationButton;
        _settingsButton.Title = TextCatalog.MainMenuSettingsButton;
    }

    private StatTile CreateTile(int index, int width, int top)
    {
        int x = ContentX + (index * (width + TileGap));

        return new StatTile { Bounds = new Rectangle(x, top, width, TileHeight) };
    }

    private Button CreateActionButton(int index, int width, int top)
    {
        int x = ContentX + (index * (width + ActionGapRow));

        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = new Rectangle(x, top, width, ActionHeight)
        };
    }

    private Button CreateSmallButton(int index, int width, int top)
    {
        int x = ContentX + (index * (width + SmallButtonGap));

        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = new Rectangle(x, top, width, Theme.SecondaryButtonHeight)
        };
    }
}
