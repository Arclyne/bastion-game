using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.MatchEnd;

// CU-25 main flow step 3. Reached today either from the opponent
// disconnected screen (a claimed win, CU-24 FA-06) or from resigning inside
// the match (CU-23); the outcome travels as the navigation argument.
public sealed class MatchEndScreen : FormScreen
{

    private const int ReasonHeight = 20;
    private const int SectionGap = 20;
    private const int TileHeight = 76;
    private const int TileGap = 14;
    private const int TileCount = 3;
    private const int EloTileIndex = 0;
    private const int ExperienceTileIndex = 1;
    private const int CoinsTileIndex = 2;
    private const int SmallButtonGap = 12;
    private const int SmallButtonCount = 2;

    private const int TilesTop = ReasonHeight + SectionGap;
    private const int SmallTop = TilesTop + TileHeight + SectionGap;
    private const int CardHeight = Theme.CardPadding + SmallTop + Theme.SecondaryButtonHeight + Theme.CardPadding;

    private readonly bool _isVictory;
    private readonly TextLine _reason;
    private readonly StatTile _eloTile;
    private readonly StatTile _experienceTile;
    private readonly StatTile _coinsTile;
    private readonly Button _watchReplayButton;
    private readonly Button _addOpponentButton;
    private readonly Button _findAnotherButton;
    private readonly Button _backMenuButton;

    public MatchEndScreen(INavigator navigator, string outcome)
        : base(navigator, WideCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(outcome);

        _isVictory = outcome == ScreenArgument.VictoryOutcome;
        int top = Card.Y + Theme.CardPadding;

        _reason = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ReasonHeight)
        };

        _eloTile = new StatTile { Bounds = GetTileBounds(EloTileIndex) };
        _experienceTile = new StatTile { Bounds = GetTileBounds(ExperienceTileIndex) };
        _coinsTile = new StatTile { Bounds = GetTileBounds(CoinsTileIndex) };

        int smallWidth = (ContentWidth - (SmallButtonGap * (SmallButtonCount - 1))) / SmallButtonCount;
        int addOpponentX = ContentX + smallWidth + SmallButtonGap;
        _watchReplayButton = CreateOutlineButton(
            new Rectangle(ContentX, top + SmallTop, smallWidth, Theme.SecondaryButtonHeight));
        _addOpponentButton = CreateOutlineButton(
            new Rectangle(addOpponentX, top + SmallTop, smallWidth, Theme.SecondaryButtonHeight));
        _watchReplayButton.Clicked += OnWatchReplayClicked;
        _addOpponentButton.Clicked += OnAddOpponentClicked;

        _findAnotherButton = CreatePrimaryButton(false);
        _backMenuButton = CreateSecondaryButton();
        _findAnotherButton.Clicked += OnFindAnotherClicked;
        _backMenuButton.Clicked += OnBackMenuClicked;

        Register(_reason);
        Register(_eloTile);
        Register(_experienceTile);
        Register(_coinsTile);
        Register(_watchReplayButton);
        Register(_addOpponentButton);
        Register(_findAnotherButton);
        Register(_backMenuButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return _isVictory ? TextCatalog.MatchEndVictoryTitle : TextCatalog.MatchEndDefeatTitle;
    }

    // Only the reason follows from how the screen was reached; the rewards
    // come from the server, so their tiles keep an empty value until then.
    protected override void ApplyTexts()
    {
        _reason.Text = _isVictory ? TextCatalog.EndReasonAbandonment : TextCatalog.EndReasonResignation;

        _eloTile.Caption = TextCatalog.MatchEndEloCaption;
        _experienceTile.Caption = TextCatalog.MatchEndExperienceCaption;
        _coinsTile.Caption = TextCatalog.MatchEndCoinsCaption;

        _watchReplayButton.Title = TextCatalog.MatchEndWatchReplayButton;
        _addOpponentButton.Title = TextCatalog.MatchEndAddOpponentButton;
        _findAnotherButton.Title = TextCatalog.MatchEndFindAnotherButton;
        _backMenuButton.Title = TextCatalog.MatchEndBackMenuButton;
    }

    private void OnWatchReplayClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Replay);
    }

    private void OnAddOpponentClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AddFriend);
    }

    private void OnFindAnotherClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.SelectMode);
    }

    private void OnBackMenuClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MainMenu);
    }

    private Rectangle GetTileBounds(int tileIndex)
    {
        int tileWidth = (ContentWidth - (TileGap * (TileCount - 1))) / TileCount;
        int x = ContentX + (tileIndex * (tileWidth + TileGap));

        return new Rectangle(x, Card.Y + Theme.CardPadding + TilesTop, tileWidth, TileHeight);
    }
}
