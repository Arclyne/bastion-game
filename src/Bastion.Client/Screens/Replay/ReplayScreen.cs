using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Replay;

// CU-37 main flow step 3. The timeline and the notation panel; the board that
// they walk through belongs to the game renderer, which does not exist yet.
public sealed class ReplayScreen : FormScreen
{
    private const int ResultHeight = 20;
    private const int SectionGap = 16;
    private const int NotationHeight = 70;
    private const int MoveLineHeight = 20;
    private const int TimelineHeight = 8;
    private const int StepButtonWidth = 150;
    private const int StepButtonGap = 12;

    private const int NotationTop = ResultHeight + SectionGap;
    private const int MoveLineTop = NotationTop + NotationHeight + SectionGap;
    private const int TimelineTop = MoveLineTop + MoveLineHeight + SectionGap;
    private const int StepsTop = TimelineTop + TimelineHeight + SectionGap;
    private const int ContentHeight = StepsTop + Theme.SecondaryButtonHeight;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _result;
    private readonly PanelBox _notationPanel;
    private readonly TextBlock _notationText;
    private readonly TextLine _moveLine;
    private readonly ProgressBar _timeline;
    private readonly Button _previousButton;
    private readonly Button _nextButton;
    private readonly Button _playButton;
    private readonly Button _exitButton;
    private bool _isPlaying;

    public ReplayScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _result = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ResultHeight)
        };

        var notationArea = new Rectangle(ContentX, top + NotationTop, ContentWidth, NotationHeight);
        _notationPanel = new PanelBox { Bounds = notationArea };
        _notationText = new TextBlock
        {
            Bounds = new Rectangle(
                notationArea.X + PanelBox.Padding,
                notationArea.Y + PanelBox.Padding,
                notationArea.Width - (PanelBox.Padding * 2),
                NotationHeight - (PanelBox.Padding * 2))
        };

        _moveLine = new TextLine
        {
            Style = TextLineStyle.Small,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top + MoveLineTop, ContentWidth, MoveLineHeight)
        };
        _timeline = new ProgressBar
        {
            Bounds = new Rectangle(ContentX, top + TimelineTop, ContentWidth, TimelineHeight)
        };

        int centerX = ContentX + ((ContentWidth - StepButtonWidth) / 2);
        int previousX = centerX - StepButtonWidth - StepButtonGap;
        int nextX = centerX + StepButtonWidth + StepButtonGap;
        _previousButton = CreateOutlineButton(
            new Rectangle(previousX, top + StepsTop, StepButtonWidth, Theme.SecondaryButtonHeight));
        _nextButton = CreateOutlineButton(
            new Rectangle(nextX, top + StepsTop, StepButtonWidth, Theme.SecondaryButtonHeight));
        _previousButton.Clicked += OnPreviousClicked;
        _nextButton.Clicked += OnNextClicked;

        _playButton = CreatePrimaryButton(false);
        _exitButton = CreateSecondaryButton();
        _playButton.Clicked += OnPlayClicked;
        _exitButton.Clicked += OnExitClicked;

        Register(_result);
        Register(_notationPanel);
        Register(_notationText);
        Register(_moveLine);
        Register(_timeline);
        Register(_previousButton);
        Register(_nextButton);
        Register(_playButton);
        Register(_exitButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ReplayTitle;
    }

    // The result, the moves and the notation come from the server, so their
    // lines stay empty and the timeline at its start until it answers.
    protected override void ApplyTexts()
    {
        _notationPanel.Title = TextCatalog.ReplayNotationTitle;

        _previousButton.Title = TextCatalog.ReplayPreviousButton;
        _nextButton.Title = TextCatalog.ReplayNextButton;
        _playButton.Title = GetPlayLabel();
        _exitButton.Title = TextCatalog.ReplayExitButton;
    }

    // Stepping needs the recorded moves, which the server does not send yet.
    private void OnPreviousClicked(object? sender, EventArgs e)
    {
    }

    private void OnNextClicked(object? sender, EventArgs e)
    {
    }

    private void OnPlayClicked(object? sender, EventArgs e)
    {
        _isPlaying = !_isPlaying;
        _playButton.Title = GetPlayLabel();
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private string GetPlayLabel()
    {
        return _isPlaying ? TextCatalog.ReplayPauseButton : TextCatalog.ReplayPlayButton;
    }
}
