using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.PlayerCard;

// CU-16 main flow step 4. Another player seen from outside: the head to head
// score, the last matches together and what can be done about them. The score
// and the matches come from the server, so the rows stay hidden until then.
public sealed class PlayerCardScreen : FormScreen
{
    private const int CardHeight = 392;
    private const int AvatarSize = 88;
    private const int SectionGap = 20;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;
    private const int ScoreWidth = 200;
    private const int ScoreHeight = 88;

    private readonly AvatarBox _avatar;
    private readonly StatTile _headToHead;
    private readonly List<DataRow> _rows = [];
    private readonly TextLine _emptyState;
    private readonly Button _addFriendButton;
    private readonly Button _watchButton;
    private readonly Button _reportButton;
    private readonly Button _backButton;

    public PlayerCardScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;
        int rowsTop = top + AvatarSize + SectionGap;

        _avatar = new AvatarBox { Bounds = new Rectangle(ContentX, top, AvatarSize, AvatarSize) };
        _headToHead = new StatTile
        {
            Bounds = new Rectangle(Card.Right - Theme.CardPadding - ScoreWidth, top, ScoreWidth, ScoreHeight)
        };

        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow { IsVisible = false, Bounds = GetRowBounds(rowIndex, rowsTop) };
            _rows.Add(row);
            Register(row);
        }

        _emptyState = new TextLine { Style = TextLineStyle.Muted, Bounds = GetRowBounds(0, rowsTop) };

        _addFriendButton = CreatePrimaryButton(false);
        _watchButton = CreateSecondaryButton();
        _reportButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_addFriendButton, _watchButton, _reportButton, _backButton]);
        _addFriendButton.Clicked += OnAddFriendClicked;
        _watchButton.Clicked += OnWatchClicked;
        _reportButton.Clicked += OnReportClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_avatar);
        Register(_headToHead);
        Register(_emptyState);
        Register(_addFriendButton);
        Register(_watchButton);
        Register(_reportButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PlayerCardSubtitle;
    }

    protected override void ApplyTexts()
    {
        _avatar.IconLabel = TextCatalog.ProfileAvatarPlaceholder;
        _headToHead.Caption = TextCatalog.PlayerCardHeadToHeadCaption;
        _emptyState.Text = TextCatalog.PlayerCardEmpty;
        _addFriendButton.Title = TextCatalog.PlayerCardAddFriendButton;
        _watchButton.Title = TextCatalog.PlayerCardWatchButton;
        _reportButton.Title = TextCatalog.PlayerCardReportButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnAddFriendClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AddFriend);
    }

    private void OnWatchClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Spectator);
    }

    private void OnReportClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Report);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private Rectangle GetRowBounds(int index, int top)
    {
        return new Rectangle(ContentX, top + (index * (RowHeight + RowGap)), ContentWidth, RowHeight);
    }
}
