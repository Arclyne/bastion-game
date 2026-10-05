using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Profile;

public sealed class ProfileScreen : FormScreen
{
    private const int AvatarSize = 96;
    private const int HeaderTextInset = AvatarSize + 20;
    private const int HeadingHeight = 30;
    private const int HeaderLineTop = 38;
    private const int BarTop = 66;
    private const int BarWidth = 300;
    private const int BarHeight = 8;
    private const int EditButtonWidth = 120;
    private const int BlockGap = 20;
    private const int TileHeight = 84;
    private const int TileGap = 16;
    private const int TileCount = 4;
    private const int MatchesTileIndex = 0;
    private const int WinsTileIndex = 1;
    private const int StreakTileIndex = 2;
    private const int TopEloTileIndex = 3;
    private const int PanelHeight = 190;
    private const int PanelRowTop = 44;
    private const int PanelRowHeight = 40;
    private const int RuleOffset = 8;
    private const int RuleHeight = 2;
    private const int ModeCount = 3;
    private const int LinksBarHeight = 64;
    private const int LinksLabelWidth = 90;
    private const int LinksLabelHeight = 22;

    private const int TilesTop = AvatarSize + BlockGap;
    private const int PanelsTop = TilesTop + TileHeight + BlockGap;
    private const int LinksTop = PanelsTop + PanelHeight + BlockGap;
    private const int ContentHeight = LinksTop + LinksBarHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + ContentHeight + Theme.CardPadding;

    private readonly Avatar _avatar;
    private readonly TextLine _nickname;
    private readonly TextLine _headerLine;
    private readonly ProgressBar _experienceBar;
    private readonly Button _editButton;
    private readonly StatTile _matchesTile;
    private readonly StatTile _winsTile;
    private readonly StatTile _streakTile;
    private readonly StatTile _topEloTile;
    private readonly PanelBox _modesPanel;
    private readonly List<TextLine> _modeNames = [];
    private readonly List<TextLine> _modeStats = [];
    private readonly PanelBox _titlesPanel;
    private readonly ChipRow _titleChips;
    private readonly PanelBox _linksBar;
    private readonly TextLine _linksLabel;
    private readonly ChipRow _linkChips;

    public ProfileScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = Theme.PanelCardWidth,
            Height = CardHeight,
            Layout = ScreenLayout.PanelBare
        })
    {
        int top = PanelContentTop;

        _avatar = new Avatar { Bounds = new Rectangle(ContentX, top, AvatarSize, AvatarSize) };
        _nickname = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX + HeaderTextInset, top, ContentWidth, HeadingHeight)
        };
        _headerLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX + HeaderTextInset, top + HeaderLineTop, ContentWidth, HeadingHeight)
        };
        _experienceBar = new ProgressBar
        {
            Bounds = new Rectangle(ContentX + HeaderTextInset, top + BarTop, BarWidth, BarHeight)
        };
        _editButton = CreateOutlineButton(
            new Rectangle(
                Card.Right - Theme.CardPadding - EditButtonWidth,
                top,
                EditButtonWidth,
                Theme.SmallButtonHeight));
        _editButton.Clicked += OnEditClicked;

        _matchesTile = CreateTile(MatchesTileIndex, top);
        _winsTile = CreateTile(WinsTileIndex, top);
        _streakTile = CreateTile(StreakTileIndex, top);
        _topEloTile = CreateTile(TopEloTileIndex, top);

        int panelWidth = (ContentWidth - BlockGap) / 2;
        var modesArea = new Rectangle(ContentX, top + PanelsTop, panelWidth, PanelHeight);
        _modesPanel = new PanelBox { Bounds = modesArea };

        for (int modeIndex = 0; modeIndex < ModeCount; modeIndex++)
        {
            Rectangle rowBounds = GetModeRowBounds(modesArea, modeIndex);
            _modeNames.Add(new TextLine { Bounds = rowBounds });
            _modeStats.Add(new TextLine
            {
                Style = TextLineStyle.Muted,
                IsRightAligned = true,
                Bounds = rowBounds
            });
        }

        var titlesArea = new Rectangle(ContentX + panelWidth + BlockGap, top + PanelsTop, panelWidth, PanelHeight);
        _titlesPanel = new PanelBox { Bounds = titlesArea };
        _titleChips = new ChipRow
        {
            IsInteractive = false,
            Bounds = new Rectangle(
                titlesArea.X + PanelBox.Padding,
                titlesArea.Y + PanelRowTop,
                titlesArea.Width - (PanelBox.Padding * 2),
                PanelHeight - PanelRowTop - PanelBox.Padding)
        };

        var linksArea = new Rectangle(ContentX, top + LinksTop, ContentWidth, LinksBarHeight);
        _linksBar = new PanelBox { Bounds = linksArea };
        _linksLabel = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(
                linksArea.X + PanelBox.Padding,
                linksArea.Y + ((LinksBarHeight - LinksLabelHeight) / 2),
                LinksLabelWidth,
                LinksLabelHeight)
        };
        _linkChips = new ChipRow
        {
            Bounds = new Rectangle(
                linksArea.X + PanelBox.Padding + LinksLabelWidth,
                linksArea.Y + ((LinksBarHeight - Theme.ChipHeight) / 2),
                linksArea.Width - (PanelBox.Padding * 2) - LinksLabelWidth,
                Theme.ChipHeight)
        };
        _linkChips.ChipChosen += OnLinkChipChosen;

        Register(_avatar);
        Register(_nickname);
        Register(_headerLine);
        Register(_experienceBar);
        Register(_editButton);
        Register(_matchesTile);
        Register(_winsTile);
        Register(_streakTile);
        Register(_topEloTile);
        Register(_modesPanel);
        RegisterModeRows(modesArea);
        Register(_titlesPanel);
        Register(_titleChips);
        Register(_linksBar);
        Register(_linksLabel);
        Register(_linkChips);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return string.Empty;
    }

    protected override void ApplyTexts()
    {
        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _editButton.Title = TextCatalog.CommonEditButton;

        _matchesTile.Caption = TextCatalog.ProfileMatchesTile;
        _winsTile.Caption = TextCatalog.ProfileWinsTile;
        _streakTile.Caption = TextCatalog.ProfileStreakTile;
        _topEloTile.Caption = TextCatalog.ProfileTopEloTile;

        _modesPanel.Title = TextCatalog.ProfileByModeTitle;
        IReadOnlyList<string> modes = GetModeNames();

        for (int modeIndex = 0; modeIndex < ModeCount; modeIndex++)
        {
            _modeNames[modeIndex].Text = modes[modeIndex];
        }

        _titlesPanel.Title = TextCatalog.ProfileTitlesTitle;
        _titlesPanel.Footer = TextCatalog.ProfileSeeAllLink;
        _titleChips.Items.Clear();

        foreach (string title in GetTitleNames())
        {
            _titleChips.Items.Add(new Chip { Text = title });
        }

        _linksLabel.Text = TextCatalog.ProfileLinksTitle;
        _linkChips.Items.Clear();
        _linkChips.Items.Add(new Chip { Text = TextCatalog.ProfileAddLinkChip, IsDashed = true });
    }

    private static IReadOnlyList<string> GetModeNames()
    {
        return [TextCatalog.GameModeClassic, TextCatalog.GameModeFourPlayers, TextCatalog.GameModeQuick];
    }

    private static IReadOnlyList<string> GetTitleNames()
    {
        return [TextCatalog.TitleRookie, TextCatalog.TitleStrategist, TextCatalog.TitleBuilder];
    }

    private static Rectangle GetModeRowBounds(Rectangle modesArea, int modeIndex)
    {
        return new Rectangle(
            modesArea.X + PanelBox.Padding,
            modesArea.Y + PanelRowTop + (modeIndex * PanelRowHeight),
            modesArea.Width - (PanelBox.Padding * 2),
            PanelRowHeight);
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnLinkChipChosen(object? sender, SelectionChangedEventArgs e)
    {
        if (e.SelectedIndex == _linkChips.Items.Count - 1)
        {
            Navigator.GoTo(ScreenId.EditProfile);
        }
    }

    private void RegisterModeRows(Rectangle modesArea)
    {
        for (int modeIndex = 0; modeIndex < ModeCount; modeIndex++)
        {
            Register(_modeNames[modeIndex]);
            Register(_modeStats[modeIndex]);
            Register(new Rule
            {
                Bounds = new Rectangle(
                    modesArea.X + PanelBox.Padding,
                    modesArea.Y + PanelRowTop + ((modeIndex + 1) * PanelRowHeight) - RuleOffset,
                    modesArea.Width - (PanelBox.Padding * 2),
                    RuleHeight)
            });
        }
    }

    private StatTile CreateTile(int index, int top)
    {
        int width = (ContentWidth - ((TileCount - 1) * TileGap)) / TileCount;
        int x = ContentX + (index * (width + TileGap));

        return new StatTile { Bounds = new Rectangle(x, top + TilesTop, width, TileHeight) };
    }
}
