using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Contracts.Leaderboard;

namespace Bastion.Client.Screens.Leaderboard;

public sealed class LeaderboardScreen : FormScreen
{
    private const int SelectorGap = 16;
    private const int FooterGap = 12;
    private const int FooterHeight = 24;
    private const int PageButtonWidth = 180;
    private const int PageButtonCount = 2;
    private const int FirstPage = 0;
    private const string OwnRowSeparator = " · ";

    private static readonly int _tableHeight = LeaderboardTable.ComputeHeight(LeaderboardRules.PageSize);
    private static readonly int _cardHeight = Theme.CardPadding + LabelSpace + Theme.ChipHeight + SelectorGap
        + _tableHeight + FooterGap + FooterHeight + Theme.CardPadding;

    private readonly LeaderboardController _controller;
    private readonly Selector _modeSelector;
    private readonly LeaderboardTable _table;
    private readonly Button _previousButton;
    private readonly Button _nextButton;
    private readonly TextLine _pageLine;
    private readonly Button _backButton;

    private Task<LeaderboardPage>? _pendingPage;
    private LeaderboardPage? _page;
    private int _pageIndex;

    public LeaderboardScreen(INavigator navigator, LeaderboardController controller)
        : base(navigator, WideCardWidth, _cardHeight)
    {
        ArgumentNullException.ThrowIfNull(controller);

        _controller = controller;
        int selectorTop = Card.Y + Theme.CardPadding + LabelSpace;
        _modeSelector = new Selector
        {
            Options = LeaderboardController.GetGameModeNames(),
            IsCompact = true,
            Bounds = new Rectangle(ContentX, selectorTop, ContentWidth, Theme.ChipHeight),
        };
        int tableTop = selectorTop + Theme.ChipHeight + SelectorGap;
        _table = new LeaderboardTable { Bounds = new Rectangle(ContentX, tableTop, ContentWidth, _tableHeight) };

        int footerTop = tableTop + _tableHeight + FooterGap;
        _previousButton = CreatePageButton(new Rectangle(ContentX, footerTop, PageButtonWidth, FooterHeight));
        _nextButton = CreatePageButton(
            new Rectangle(Card.Right - Theme.CardPadding - PageButtonWidth, footerTop, PageButtonWidth, FooterHeight));
        _pageLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(
                ContentX + PageButtonWidth,
                footerTop,
                ContentWidth - (PageButtonWidth * PageButtonCount),
                FooterHeight),
        };
        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);

        _modeSelector.SelectionChanged += OnModeSelected;
        _previousButton.Clicked += OnPreviousClicked;
        _nextButton.Clicked += OnNextClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_modeSelector);
        Register(_table);
        Register(_previousButton);
        Register(_nextButton);
        Register(_pageLine);
        Register(_backButton);

        ApplyTexts();
        Load(FirstPage);
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_pendingPage is { IsCompleted: true } completed)
        {
            _pendingPage = null;
            _page = completed.GetAwaiter().GetResult();
            ShowPage();
        }
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LeaderboardSubtitle;
    }

    protected override void ApplyTexts()
    {
        _modeSelector.Label = TextCatalog.LeaderboardModeLabel;
        _modeSelector.Options = LeaderboardController.GetGameModeNames();
        _table.Headers =
        [
            TextCatalog.LeaderboardPositionHeader,
            TextCatalog.LeaderboardPlayerHeader,
            TextCatalog.LeaderboardEloHeader,
            TextCatalog.LeaderboardMatchesHeader,
            TextCatalog.LeaderboardWinsHeader,
        ];
        _previousButton.Title = TextCatalog.LeaderboardPreviousButton;
        _nextButton.Title = TextCatalog.LeaderboardNextButton;
        _backButton.Title = TextCatalog.CommonBackButton;
        ShowPage();
    }

    private void Load(int pageIndex)
    {
        _pageIndex = pageIndex;
        _page = null;
        string gameModeCode = LeaderboardController.GameModeCodes[_modeSelector.SelectedIndex];
        _pendingPage = _controller.LoadAsync(gameModeCode, pageIndex);
        ShowPage();
    }

    private void ShowPage()
    {
        _pageLine.Text = LeaderboardController.GetPageText(_pageIndex);
        if (_page is null)
        {
            ShowStatus(TextCatalog.LeaderboardLoading);
            return;
        }

        string? status = LeaderboardController.GetStatusText(_page);
        _table.StatusText = status ?? string.Empty;
        _table.Rows = _page.Entries.Select(LeaderboardController.ToView).ToList();
        _table.OwnRow = _page.OwnEntry is null ? null : LeaderboardController.ToView(_page.OwnEntry);
        _table.OwnRowNote = GetOwnRowNote(_page);
        _previousButton.IsEnabled = _pageIndex > FirstPage;
        _nextButton.IsEnabled = _page.HasNextPage;
    }

    private void ShowStatus(string status)
    {
        _table.StatusText = status;
        _table.Rows = new List<LeaderboardRowView>();
        _table.OwnRow = null;
        _table.OwnRowNote = string.Empty;
        _previousButton.IsEnabled = false;
        _nextButton.IsEnabled = false;
    }

    private string GetOwnRowNote(LeaderboardPage page)
    {
        if (page.OwnMatchesToQualify > 0)
        {
            string notRanked = LeaderboardController.GetNotRankedText(page.OwnMatchesToQualify);
            return _controller.OwnNickname + OwnRowSeparator + notRanked;
        }

        return _controller.OwnNickname;
    }

    private void OnModeSelected(object? sender, SelectionChangedEventArgs e)
    {
        Load(FirstPage);
    }

    private void OnPreviousClicked(object? sender, EventArgs e)
    {
        if (_pendingPage is null && _pageIndex > FirstPage)
        {
            Load(_pageIndex - 1);
        }
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
        if (_pendingPage is null && _page is { HasNextPage: true })
        {
            Load(_pageIndex + 1);
        }
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private static Button CreatePageButton(Rectangle bounds)
    {
        return new Button { Style = ButtonStyle.Link, Bounds = bounds };
    }
}
