using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.WaitingRoom;

// CU-18 main flow step 4 (host) and CU-19 main flow step 3 (guest). One class
// for both roles: the host waits with the start button disabled until the
// rest are ready, the guest gets a ready toggle instead.
public sealed class WaitingRoomScreen : FormScreen
{

    private const int CodeValueWidth = 220;
    private const int CodeLabelHeight = 20;
    private const int CodeLabelGap = 4;
    private const int CodeValueTop = CodeLabelHeight + CodeLabelGap;
    private const int SectionGap = 20;
    private const int SeatCount = 2;
    private const int OwnSeatIndex = 0;
    private const int OtherSeatIndex = 1;
    private const int SeatHeight = 54;
    private const int SeatGap = 8;
    private const int ChatLineHeight = 20;
    private const int ChatLineGap = 4;

    private const int SeatsLabelTop = CodeValueTop + Theme.FieldHeight + SectionGap;
    private const int SeatsTop = SeatsLabelTop + LabelSpace;
    private const int ChatTitleTop = SeatsTop + (SeatCount * (SeatHeight + SeatGap)) + SectionGap;
    private const int ChatLineTop = ChatTitleTop + ChatLineHeight + ChatLineGap;
    private const int ChatFieldTop = ChatLineTop + ChatLineHeight + ChatLineGap;
    private const int CardHeight = Theme.CardPadding + ChatFieldTop + Theme.FieldHeight + Theme.CardPadding;

    private readonly bool _isHost;
    private readonly TextLine _codeLabel;
    private readonly ValueBox _codeValue;
    private readonly TextLine _seatsLabel;
    private readonly DataRow _ownSeat;
    private readonly DataRow _otherSeat;
    private readonly TextLine _chatTitle;
    private readonly TextLine _chatLine;
    private readonly TextField _chatField;
    private readonly Button _actionButton;
    private readonly Button _leaveButton;
    private bool _isReady;

    public WaitingRoomScreen(INavigator navigator, string role)
        : base(navigator, WideCardWidth, CardHeight)
    {
        ArgumentNullException.ThrowIfNull(role);

        _isHost = role == ScreenArgument.HostRole;
        int top = Card.Y + Theme.CardPadding;

        _codeLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, CodeLabelHeight)
        };
        _codeValue = CreateValueBox(
            new Rectangle(ContentX, top + CodeValueTop, CodeValueWidth, Theme.FieldHeight));

        _seatsLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + SeatsLabelTop, ContentWidth, LabelSpace)
        };
        _ownSeat = new DataRow
        {
            IsHighlighted = true,
            Bounds = GetSeatBounds(OwnSeatIndex)
        };
        _otherSeat = new DataRow { Bounds = GetSeatBounds(OtherSeatIndex) };

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

        _actionButton = CreatePrimaryButton(false);
        _leaveButton = CreateSecondaryButton();
        _actionButton.IsEnabled = !_isHost;
        _actionButton.Clicked += OnActionClicked;
        _leaveButton.Clicked += OnLeaveClicked;

        Register(_ownSeat);
        Register(_otherSeat);
        Register(_codeLabel);
        Register(_codeValue);
        Register(_seatsLabel);
        Register(_chatTitle);
        Register(_chatLine);
        RegisterField(_chatField);
        Register(_actionButton);
        Register(_leaveButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.WaitingRoomSubtitle;
    }

    // The room code, the nicknames and the chat come from the server, so
    // until it answers only the texts tied to the player's role are shown.
    protected override void ApplyTexts()
    {
        _codeLabel.Text = TextCatalog.WaitingRoomCodeLabel;
        _seatsLabel.Text = TextCatalog.WaitingRoomPlayersLabel;

        _ownSeat.Value = _isHost ? TextCatalog.WaitingRoomHostTag : TextCatalog.WaitingRoomYouTag;

        _otherSeat.Title = _isHost ? TextCatalog.WaitingRoomFreeSlot : string.Empty;
        _otherSeat.Subtitle = _isHost ? TextCatalog.WaitingRoomHostNote : string.Empty;
        _otherSeat.Value = _isHost ? string.Empty : TextCatalog.WaitingRoomHostTag;

        _chatTitle.Text = TextCatalog.MatchChatTitle;
        _chatField.Placeholder = TextCatalog.WaitingRoomChatPlaceholder;

        _actionButton.Title = GetActionLabel();
        _leaveButton.Title = TextCatalog.WaitingRoomLeaveButton;
    }

    private void OnActionClicked(object? sender, EventArgs e)
    {
        if (_isHost)
        {
            Navigator.GoTo(ScreenId.VersusScreen);
            return;
        }

        _isReady = !_isReady;
        _actionButton.Title = GetActionLabel();
    }

    private void OnLeaveClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.WaitingRoomLeaveBody,
            PrimaryLabel = TextCatalog.WaitingRoomLeaveButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OnLeaveConfirmed
        });
    }

    private void OnLeaveConfirmed()
    {
        Navigator.GoBack();
    }

    private string GetActionLabel()
    {
        if (_isHost)
        {
            return TextCatalog.WaitingRoomStartButton;
        }

        return _isReady ? TextCatalog.CommonCancelButton : TextCatalog.WaitingRoomReadyButton;
    }

    private Rectangle GetSeatBounds(int seatIndex)
    {
        int seatTop = Card.Y + Theme.CardPadding + SeatsTop + (seatIndex * (SeatHeight + SeatGap));

        return new Rectangle(ContentX, seatTop, ContentWidth, SeatHeight);
    }
}
