using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Matchmaking;

// CU-17 main flow step 4. The use case draws this as a panel over the menu
// that leaves the rest of the game reachable (RN-08); the navigator has no
// concept yet of a screen running behind another, so this is a full screen
// with the one control that matters: cancel.
public sealed class MatchmakingScreen : FormScreen
{
    private const int TitleHeight = 28;
    private const int ElapsedTop = 36;
    private const int ElapsedHeight = 22;
    private const int NoteGap = 14;
    private const int NoteTop = ElapsedTop + ElapsedHeight + NoteGap;
    private const int NoteHeight = 40;
    private const int CardHeight = Theme.CardPadding + NoteTop + NoteHeight + Theme.CardPadding;
    private const float NotStarted = -1f;
    private const string ClockPattern = @"m\:ss";

    // Stands in for the server answering with a rival. The wait is what the
    // player sees; the number goes away with the matchmaking request.
    private const float SearchSeconds = 4f;

    private readonly TextLine _title;
    private readonly TextLine _elapsed;
    private readonly TextBlock _note;
    private readonly Button _cancelButton;
    private float _startedAt = NotStarted;

    public MatchmakingScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _title = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, TitleHeight)
        };
        _elapsed = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top + ElapsedTop, ContentWidth, ElapsedHeight)
        };
        _note = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, top + NoteTop, ContentWidth, NoteHeight)
        };

        _cancelButton = CreateSecondaryButton();
        _cancelButton.Clicked += OnCancelClicked;

        Register(_title);
        Register(_elapsed);
        Register(_note);
        Register(_cancelButton);

        ApplyTexts();
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_startedAt < 0f)
        {
            _startedAt = input.ElapsedSeconds;
        }

        float elapsed = input.ElapsedSeconds - _startedAt;
        string clock = TimeSpan.FromSeconds(elapsed).ToString(ClockPattern);
        _elapsed.Text = string.Format(TextCatalog.MatchmakingElapsedFormat, clock);

        if (elapsed >= SearchSeconds)
        {
            Navigator.GoTo(ScreenId.VersusScreen);
        }
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MatchmakingSubtitle;
    }

    protected override void ApplyTexts()
    {
        _title.Text = TextCatalog.MatchmakingTitle;
        _note.Text = TextCatalog.MatchmakingBrowseNote;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
