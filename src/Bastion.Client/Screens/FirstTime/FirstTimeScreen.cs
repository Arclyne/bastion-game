using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.FirstTime;

// CU-08 main flow step 1. The one-time choice of pawn and icon (D-13); the
// nickname was already fixed at registration and only shown here.
public sealed class FirstTimeScreen : FormScreen
{
    private const int IconRows = 2;
    private const int IconRowGap = 8;

    private const int LabelHeight = LabelSpace;
    private const int SectionGap = 18;
    private const int NoticeHeight = 40;
    private const int IconChipsHeight = (IconRows * Theme.ChipHeight) + ((IconRows - 1) * IconRowGap);

    private const int IconLabelTop = LabelHeight + Theme.ChipHeight + SectionGap;
    private const int IconChipsTop = IconLabelTop + LabelHeight;
    private const int NicknameTop = IconChipsTop + IconChipsHeight + SectionGap + LabelHeight;
    private const int NoticeTop = NicknameTop + Theme.FieldHeight + SectionGap;
    private const int CardHeight = Theme.CardPadding + NoticeTop + NoticeHeight + Theme.CardPadding;

    private readonly TextLine _pawnLabel;
    private readonly ChipRow _pawnChips;
    private readonly TextLine _iconLabel;
    private readonly ChipRow _iconChips;
    private readonly ValueBox _nicknameBox;
    private readonly TextBlock _notice;
    private readonly Button _startButton;
    private readonly Button _skipButton;
    private int _pawnIndex;
    private int _iconIndex;

    public FirstTimeScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _pawnLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, LabelHeight)
        };
        _pawnChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + LabelHeight, ContentWidth, Theme.ChipHeight)
        };
        _pawnChips.ChipChosen += OnPawnChosen;

        _iconLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top + IconLabelTop, ContentWidth, LabelHeight)
        };
        _iconChips = new ChipRow
        {
            Bounds = new Rectangle(ContentX, top + IconChipsTop, ContentWidth, IconChipsHeight)
        };
        _iconChips.ChipChosen += OnIconChosen;

        _nicknameBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, top + NicknameTop, ColumnWidth, Theme.FieldHeight)
        };
        _notice = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, top + NoticeTop, ContentWidth, NoticeHeight)
        };

        _startButton = CreatePrimaryButton(true);
        _skipButton = CreateSecondaryButton();
        _startButton.Clicked += OnStartClicked;
        _skipButton.Clicked += OnSkipClicked;

        Register(_pawnLabel);
        Register(_pawnChips);
        Register(_iconLabel);
        Register(_iconChips);
        Register(_nicknameBox);
        Register(_notice);
        Register(_startButton);
        Register(_skipButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.FirstTimeSubtitle;
    }

    protected override void ApplyTexts()
    {
        _pawnLabel.Text = TextCatalog.FirstTimePawnLabel;
        RefreshPawnChips();

        _iconLabel.Text = TextCatalog.FirstTimeIconLabel;
        RefreshIconChips();

        _nicknameBox.Label = TextCatalog.FirstTimeNicknameLabel;
        _notice.Text = TextCatalog.FirstTimeNotice;

        _startButton.Title = TextCatalog.FirstTimeStartButton;
        _skipButton.Title = TextCatalog.FirstTimeSkipButton;
    }

    private static void FillChips(ChipRow row, string[] texts, int selectedIndex)
    {
        row.Items.Clear();

        for (int chipIndex = 0; chipIndex < texts.Length; chipIndex++)
        {
            row.Items.Add(new Chip { Text = texts[chipIndex], IsSelected = chipIndex == selectedIndex });
        }
    }

    private void OnPawnChosen(object? sender, SelectionChangedEventArgs e)
    {
        _pawnIndex = e.SelectedIndex;
        RefreshPawnChips();
    }

    private void OnIconChosen(object? sender, SelectionChangedEventArgs e)
    {
        _iconIndex = e.SelectedIndex;
        RefreshIconChips();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.MainMenu);
    }

    private void OnSkipClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.LinkAccount);
    }

    private void RefreshPawnChips()
    {
        string[] pawns = [TextCatalog.PawnClassic, TextCatalog.PawnStone];
        FillChips(_pawnChips, pawns, _pawnIndex);
    }

    private void RefreshIconChips()
    {
        string[] icons =
        [
            TextCatalog.IconFox,
            TextCatalog.IconOwl,
            TextCatalog.IconBear,
            TextCatalog.IconCat,
            TextCatalog.IconWall,
            TextCatalog.IconPawn,
            TextCatalog.IconLighthouse
        ];
        FillChips(_iconChips, icons, _iconIndex);
    }
}
