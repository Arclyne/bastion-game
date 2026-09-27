using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Match;

// CU-17 main flow step 7. The board itself belongs to the game renderer,
// which does not exist yet; this is the frame around it: whose turn it is,
// the two actions that are neither a move nor a wall (CU-22, CU-23), and the
// match chat.
public sealed class MatchScreen : FormScreen
{

    private const int HeaderHeight = 22;
    private const int ClockNudge = 2;
    private const int SectionGap = 14;
    private const int PlayerRowCount = 2;
    private const int PlayerRowHeight = 46;
    private const int PlayerRowGap = 8;
    private const int BoardHeight = 90;
    private const int ChatTitleHeight = 20;
    private const int ChatLineHeight = 18;
    private const int ChatLineGap = 4;

    private const int PlayersTop = HeaderHeight + SectionGap;
    private const int ChipsTop = PlayersTop + (PlayerRowHeight * PlayerRowCount) + PlayerRowGap + SectionGap;
    private const int BoardTop = ChipsTop + Theme.ChipHeight + SectionGap;
    private const int ChatTop = BoardTop + BoardHeight + SectionGap;
    private const int ChatFieldTop = ChatTop + ChatTitleHeight + ChatLineGap + ChatLineHeight + ChatLineGap;
    private const int CardHeight = Theme.CardPadding + ChatFieldTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly TextLine _turnLabel;
    private readonly TextLine _clockLabel;
    private readonly DataRow _selfRow;
    private readonly DataRow _rivalRow;
    private readonly ChipRow _actionChips;
    private readonly PanelBox _board;
    private readonly TextLine _chatTitle;
    private readonly TextLine _chatLine;
    private readonly TextField _chatField;
    private readonly Button _drawButton;
    private readonly Button _resignButton;
    private readonly bool _isAgainstAi;
    private int _actionIndex;

    public MatchScreen(INavigator navigator, string? opponent)
        : base(navigator, WideCardWidth, CardHeight)
    {
        _isAgainstAi = opponent == ScreenArgument.AiOpponent;

        int top = Card.Y + Theme.CardPadding;

        _turnLabel = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HeaderHeight)
        };
        _clockLabel = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsRightAligned = true,
            Bounds = new Rectangle(ContentX, top + ClockNudge, ContentWidth, HeaderHeight)
        };

        _selfRow = new DataRow
        {
            IsHighlighted = true,
            Bounds = new Rectangle(ContentX, top + PlayersTop, ContentWidth, PlayerRowHeight)
        };
        int rivalRowTop = top + PlayersTop + PlayerRowHeight + PlayerRowGap;
        _rivalRow = new DataRow
        {
            Bounds = new Rectangle(ContentX, rivalRowTop, ContentWidth, PlayerRowHeight)
        };

        _actionChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + ChipsTop, ContentWidth, Theme.ChipHeight)
        };
        _actionChips.ChipChosen += OnActionChosen;

        _board = new PanelBox
        {
            IsDashed = true,
            Bounds = new Rectangle(ContentX, top + BoardTop, ContentWidth, BoardHeight)
        };

        int chatLineTop = top + ChatTop + ChatTitleHeight + ChatLineGap;
        _chatTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ChatTop, ContentWidth, ChatTitleHeight)
        };
        _chatLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, chatLineTop, ContentWidth, ChatLineHeight)
        };
        _chatField = new TextField
        {
            Bounds = new Rectangle(ContentX, top + ChatFieldTop, ContentWidth, Theme.FieldHeight)
        };

        _drawButton = CreateSecondaryButton();
        _resignButton = CreatePrimaryButton(false);
        LayOutActionsInRow([_drawButton, _resignButton]);
        _drawButton.Clicked += OnDrawClicked;
        _resignButton.Clicked += OnResignClicked;

        Register(_turnLabel);
        Register(_clockLabel);
        Register(_selfRow);
        Register(_rivalRow);
        Register(_actionChips);
        Register(_board);
        Register(_chatTitle);
        Register(_chatLine);
        RegisterField(_chatField);
        Register(_drawButton);
        Register(_resignButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MatchSubtitle;
    }

    // The turn, the clock, the players, their walls and the chat come from the
    // server, so their lines stay empty until it answers.
    protected override void ApplyTexts()
    {
        RefreshActionChips();
        _board.Title = TextCatalog.MatchBoardPlaceholder;

        _chatTitle.Text = TextCatalog.MatchChatTitle;
        _chatField.Placeholder = TextCatalog.MatchChatPlaceholder;

        _drawButton.Title = TextCatalog.MatchDrawButton;
        _resignButton.Title = TextCatalog.MatchResignButton;
    }

    private void OnActionChosen(object? sender, SelectionChangedEventArgs e)
    {
        _actionIndex = e.SelectedIndex;
        RefreshActionChips();
    }

    private void OnDrawClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.MatchDrawBody,
            PrimaryLabel = TextCatalog.MatchDrawConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnDrawOffered
        });
    }

    // The offer waits for the rival. Until the server is there to answer it,
    // the wait resolves the way D-15 describes: the rival stopped answering.
    private void OnDrawOffered()
    {
        Navigator.GoTo(ScreenId.OpponentDisconnected);
    }

    private void OnResignClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.MatchResignBody,
            PrimaryLabel = TextCatalog.MatchResignConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnResignConfirmed
        });
    }

    private void OnResignConfirmed()
    {
        if (_isAgainstAi)
        {
            Navigator.GoTo(ScreenId.AIMatchEnd);
            return;
        }

        Navigator.GoTo(ScreenId.MatchEnd, ScreenArgument.DefeatOutcome);
    }

    private void RefreshActionChips()
    {
        _actionChips.Items.Clear();
        string[] actions = [TextCatalog.MatchMoveChip, TextCatalog.MatchWallChip];

        for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
        {
            _actionChips.Items.Add(new Chip { Text = actions[actionIndex], IsSelected = actionIndex == _actionIndex });
        }
    }
}
