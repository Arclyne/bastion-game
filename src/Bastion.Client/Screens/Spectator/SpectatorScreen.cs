using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Spectator;

// CU-34 main flow step 3. A free camera on someone else's match, with the
// eight second delay D-19 fixes so a spectator can never race the players.
public sealed class SpectatorScreen : FormScreen
{
    private const int DelaySeconds = 8;
    private const int MoveRowCount = 3;

    private const int StatusHeight = 22;
    private const int SectionGap = 16;
    private const int MovesLabelHeight = LabelSpace;
    private const int MoveRowHeight = 34;
    private const int MoveRowGap = 6;
    private const int ChatLineHeight = 20;
    private const int ChatLineGap = 4;

    private const int MovesLabelTop = StatusHeight + SectionGap;
    private const int MovesTop = MovesLabelTop + MovesLabelHeight;
    private const int ChatTitleTop = MovesTop + (MoveRowCount * (MoveRowHeight + MoveRowGap)) + SectionGap;
    private const int ChatLineTop = ChatTitleTop + ChatLineHeight + ChatLineGap;
    private const int ChatFieldTop = ChatLineTop + ChatLineHeight + ChatLineGap;
    private const int CardHeight = Theme.CardPadding + ChatFieldTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly TextLine _liveTag;
    private readonly TextLine _viewers;
    private readonly TextLine _delay;
    private readonly TextLine _movesLabel;
    private readonly List<DataRow> _moves = [];
    private readonly TextLine _chatTitle;
    private readonly TextLine _chatLine;
    private readonly TextField _chatField;
    private readonly Button _leaveButton;

    public SpectatorScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _liveTag = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, StatusHeight)
        };
        _viewers = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsRightAligned = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, StatusHeight)
        };
        _delay = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(ContentX, top + LabelSpace, ContentWidth, StatusHeight - LabelSpace)
        };

        _movesLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + MovesLabelTop, ContentWidth, MovesLabelHeight)
        };

        for (int moveIndex = 0; moveIndex < MoveRowCount; moveIndex++)
        {
            int rowTop = top + MovesTop + (moveIndex * (MoveRowHeight + MoveRowGap));
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = new Rectangle(ContentX, rowTop, ContentWidth, MoveRowHeight)
            };
            _moves.Add(row);
            Register(row);
        }

        _chatTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + ChatTitleTop, ContentWidth, ChatLineHeight)
        };
        _chatLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, top + ChatLineTop, ContentWidth, ChatLineHeight)
        };
        _chatField = new TextField
        {
            Bounds = new Rectangle(ContentX, top + ChatFieldTop, ContentWidth, Theme.FieldHeight)
        };

        _leaveButton = CreatePrimaryButton(false);
        _leaveButton.Clicked += OnLeaveClicked;

        Register(_liveTag);
        Register(_viewers);
        Register(_delay);
        Register(_movesLabel);
        Register(_chatTitle);
        Register(_chatLine);
        RegisterField(_chatField);
        Register(_leaveButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.SpectatorSubtitle;
    }

    // The viewer count, the moves and the chat come from the server, so their
    // lines stay empty and the move rows hidden until it answers.
    protected override void ApplyTexts()
    {
        _liveTag.Text = TextCatalog.SpectatorLiveTag;
        _delay.Text = string.Format(TextCatalog.SpectatorDelayFormat, DelaySeconds);
        _movesLabel.Text = TextCatalog.SpectatorMovesTitle;

        _chatTitle.Text = TextCatalog.SpectatorChatTitle;
        _chatField.Placeholder = TextCatalog.SpectatorChatPlaceholder;

        _leaveButton.Title = TextCatalog.SpectatorLeaveButton;
    }

    private void OnLeaveClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
