using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.PrivateMatch;

public sealed class PrivateMatchScreen : FormScreen
{
    private const int RoomCodeLength = 6;

    private const int ColumnGap = Gutter;
    private const int TitleHeight = 22;
    private const int TitleGap = 8;
    private const int HintTop = TitleHeight + TitleGap;
    private const int HintHeight = 40;
    private const int HintGap = 16;
    private const int ActionTop = HintTop + HintHeight + HintGap;
    private const int CodeLabelTop = HintTop + HintHeight + HintGap;
    private const int CodeFieldTop = CodeLabelTop + LabelSpace;
    private const int JoinButtonTop = CodeFieldTop + Theme.FieldHeight + Theme.WarningSpace;
    private const int ContentHeight = JoinButtonTop + Theme.PanelButtonHeight;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _createTitle;
    private readonly TextBlock _createHint;
    private readonly Button _createButton;
    private readonly TextLine _joinTitle;
    private readonly TextField _codeField;
    private readonly Button _joinButton;
    private readonly Button _backButton;

    public PrivateMatchScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;
        int columnWidth = (ContentWidth - ColumnGap) / 2;
        int leftX = ContentX;
        int rightX = ContentX + columnWidth + ColumnGap;

        _createTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(leftX, top, columnWidth, TitleHeight)
        };
        _createHint = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(leftX, top + HintTop, columnWidth, HintHeight)
        };
        _createButton = CreateOutlineButton(
            new Rectangle(leftX, top + ActionTop, columnWidth, Theme.PanelButtonHeight));
        _createButton.Clicked += OnCreateClicked;

        _joinTitle = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(rightX, top, columnWidth, TitleHeight)
        };
        _codeField = new TextField
        {
            MaxLength = RoomCodeLength,
            IsCentered = true,
            Bounds = new Rectangle(rightX, top + CodeFieldTop, columnWidth, Theme.FieldHeight)
        };
        _joinButton = CreateOutlineButton(
            new Rectangle(rightX, top + JoinButtonTop, columnWidth, Theme.PanelButtonHeight));
        _joinButton.Clicked += OnJoinClicked;
        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;

        Register(_createTitle);
        Register(_createHint);
        Register(_createButton);
        Register(_joinTitle);
        RegisterField(_codeField);
        Register(_joinButton);
        Register(_backButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PrivateMatchSubtitle;
    }

    protected override void ApplyTexts()
    {
        _createTitle.Text = TextCatalog.PrivateMatchCreateTitle;
        _createHint.Text = TextCatalog.PrivateMatchCreateHint;
        _createButton.Title = TextCatalog.PrivateMatchCreateButton;

        _joinTitle.Text = TextCatalog.PrivateMatchJoinTitle;
        _codeField.Label = TextCatalog.PrivateMatchCodeLabel;
        _codeField.Placeholder = TextCatalog.PrivateMatchCodePlaceholder;
        _joinButton.Title = TextCatalog.PrivateMatchJoinButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnCreateClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.WaitingRoom, ScreenArgument.HostRole);
    }

    private void OnJoinClicked(object? sender, EventArgs e)
    {
        string code = _codeField.Text.Trim();
        _codeField.Warning = code.Length == RoomCodeLength ? null : TextCatalog.PrivateMatchCodeRequired;

        if (_codeField.HasWarning)
        {
            return;
        }

        Navigator.GoTo(ScreenId.WaitingRoom, ScreenArgument.GuestRole);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
