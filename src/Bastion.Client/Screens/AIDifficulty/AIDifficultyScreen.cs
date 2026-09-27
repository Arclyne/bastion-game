using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.AIDifficulty;

// CU-27 main flow step 1. Four levels, the board size, the clock and the two
// assist switches, all local to the client until the AI opponent exists.
public sealed class AIDifficultyScreen : FormScreen
{

    private const int LabelHeight = LabelSpace;
    private const int SectionGap = 18;
    private const int StrengthGap = 6;
    private const int StrengthHeight = 20;
    private const int ToggleCount = 2;
    private const int ToggleHeight = 40;
    private const int ToggleGap = 8;

    private const int LevelChipsTop = LabelHeight;
    private const int StrengthTop = LevelChipsTop + Theme.ChipHeight + StrengthGap;
    private const int BoardLabelTop = StrengthTop + StrengthHeight + SectionGap;
    private const int BoardChipsTop = BoardLabelTop + LabelHeight;
    private const int ClockLabelTop = BoardChipsTop + Theme.ChipHeight + SectionGap;
    private const int ClockChipsTop = ClockLabelTop + LabelHeight;
    private const int TogglesTop = ClockChipsTop + Theme.ChipHeight + SectionGap;
    private const int HintToggleTop = TogglesTop + ToggleHeight + ToggleGap;
    private const int ContentHeight = TogglesTop + (ToggleHeight * ToggleCount) + ToggleGap;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private static readonly int[] _clockMinutes = [3, 5, 10];
    private static readonly int[] _levelStrengths = [800, 1200, 1600, 2000];

    private readonly TextLine _levelLabel;
    private readonly ChipRow _levelChips;
    private readonly TextLine _strengthLine;
    private readonly TextLine _boardLabel;
    private readonly ChipRow _boardChips;
    private readonly TextLine _clockLabel;
    private readonly ChipRow _clockChips;
    private readonly ToggleSwitch _undoToggle;
    private readonly ToggleSwitch _hintToggle;
    private readonly Button _playButton;
    private readonly Button _backButton;
    private int _levelIndex;
    private int _boardIndex;
    private int _clockIndex;

    public AIDifficultyScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _levelLabel = CreateLabel(top);
        _levelChips = CreateChipRow(top + LevelChipsTop);
        _levelChips.ChipChosen += OnLevelChosen;
        _strengthLine = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(ContentX, top + StrengthTop, ContentWidth, StrengthHeight)
        };

        _boardLabel = CreateLabel(top + BoardLabelTop);
        _boardChips = CreateChipRow(top + BoardChipsTop);
        _boardChips.ChipChosen += OnBoardChosen;

        _clockLabel = CreateLabel(top + ClockLabelTop);
        _clockChips = CreateChipRow(top + ClockChipsTop);
        _clockChips.ChipChosen += OnClockChosen;

        _undoToggle = new ToggleSwitch
        {
            IsOn = true,
            Bounds = new Rectangle(ContentX, top + TogglesTop, ContentWidth, ToggleHeight)
        };
        _hintToggle = new ToggleSwitch
        {
            IsOn = true,
            Bounds = new Rectangle(ContentX, top + HintToggleTop, ContentWidth, ToggleHeight)
        };

        _playButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _playButton.Clicked += OnPlayClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_levelLabel);
        Register(_levelChips);
        Register(_strengthLine);
        Register(_boardLabel);
        Register(_boardChips);
        Register(_clockLabel);
        Register(_clockChips);
        Register(_undoToggle);
        Register(_hintToggle);
        Register(_playButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AIDifficultySubtitle;
    }

    protected override void ApplyTexts()
    {
        _levelLabel.Text = TextCatalog.AIDifficultyLevelLabel;
        RefreshLevelChips();

        _boardLabel.Text = TextCatalog.AIDifficultyBoardLabel;
        RefreshBoardChips();

        _clockLabel.Text = TextCatalog.AIDifficultyClockLabel;
        RefreshClockChips();

        _undoToggle.Text = TextCatalog.AIDifficultyUndoToggle;
        _hintToggle.Text = TextCatalog.AIDifficultyHintToggle;
        _playButton.Title = TextCatalog.AIDifficultyPlayButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private TextLine CreateLabel(int top)
    {
        return new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, LabelHeight)
        };
    }

    private ChipRow CreateChipRow(int top)
    {
        return new ChipRow { Bounds = new Rectangle(ContentX, top, ContentWidth, Theme.ChipHeight) };
    }

    private void OnLevelChosen(object? sender, SelectionChangedEventArgs e)
    {
        _levelIndex = e.SelectedIndex;
        RefreshLevelChips();
    }

    private void OnBoardChosen(object? sender, SelectionChangedEventArgs e)
    {
        _boardIndex = e.SelectedIndex;
        RefreshBoardChips();
    }

    private void OnClockChosen(object? sender, SelectionChangedEventArgs e)
    {
        _clockIndex = e.SelectedIndex;
        RefreshClockChips();
    }

    private void OnPlayClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Match, ScreenArgument.AiOpponent);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void RefreshLevelChips()
    {
        _levelChips.Items.Clear();
        string[] levels =
        [
            TextCatalog.AiLevelApprentice,
            TextCatalog.AiLevelBuilder,
            TextCatalog.AiLevelArchitect,
            TextCatalog.AiLevelBastion
        ];

        for (int levelIndex = 0; levelIndex < levels.Length; levelIndex++)
        {
            _levelChips.Items.Add(new Chip { Text = levels[levelIndex], IsSelected = levelIndex == _levelIndex });
        }

        _strengthLine.Text = string.Format(TextCatalog.AIDifficultyStrengthFormat, _levelStrengths[_levelIndex]);
    }

    private void RefreshBoardChips()
    {
        _boardChips.Items.Clear();
        string[] boards = [TextCatalog.AIDifficultyBoardClassicChip, TextCatalog.AIDifficultyBoardRapidChip];

        for (int boardIndex = 0; boardIndex < boards.Length; boardIndex++)
        {
            _boardChips.Items.Add(new Chip { Text = boards[boardIndex], IsSelected = boardIndex == _boardIndex });
        }
    }

    private void RefreshClockChips()
    {
        _clockChips.Items.Clear();

        for (int clockIndex = 0; clockIndex < _clockMinutes.Length; clockIndex++)
        {
            string text = string.Format(TextCatalog.AIDifficultyClockChipFormat, _clockMinutes[clockIndex]);
            _clockChips.Items.Add(new Chip { Text = text, IsSelected = clockIndex == _clockIndex });
        }

        _clockChips.Items.Add(new Chip
        {
            Text = TextCatalog.AIDifficultyNoClockChip,
            IsSelected = _clockIndex == _clockMinutes.Length
        });
    }
}
