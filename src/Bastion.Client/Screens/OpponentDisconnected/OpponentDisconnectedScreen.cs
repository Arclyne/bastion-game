using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.OpponentDisconnected;

public sealed class OpponentDisconnectedScreen : FormScreen
{
    private const float ClaimDelaySeconds = 60f;
    private const float NotStarted = -1f;
    private const string ClockPattern = @"m\:ss";
    private const int ElapsedHeight = 22;
    private const int HintGap = 14;
    private const int HintTop = ElapsedHeight + HintGap;
    private const int HintHeight = 40;
    private const int CardHeight = Theme.CardPadding + HintTop + HintHeight + Theme.CardPadding;

    private readonly TextLine _elapsed;
    private readonly TextBlock _hint;
    private readonly Button _claimButton;
    private readonly Button _backButton;
    private float _startedAt = NotStarted;

    public OpponentDisconnectedScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _elapsed = new TextLine
        {
            Style = TextLineStyle.Heading,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ElapsedHeight)
        };
        _hint = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, top + HintTop, ContentWidth, HintHeight)
        };

        _claimButton = CreatePrimaryButton(false);
        _backButton = CreateSecondaryButton();
        _claimButton.IsEnabled = false;
        _claimButton.Clicked += OnClaimClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_elapsed);
        Register(_hint);
        Register(_claimButton);
        Register(_backButton);

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
        _elapsed.Text = string.Format(TextCatalog.OpponentDisconnectedElapsedFormat, clock);
        _claimButton.IsEnabled = elapsed >= ClaimDelaySeconds;
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.OpponentDisconnectedTitle;
    }

    protected override void ApplyTexts()
    {
        _hint.Text = TextCatalog.OpponentDisconnectedHint;
        _claimButton.Title = TextCatalog.OpponentDisconnectedClaimButton;
        _backButton.Title = TextCatalog.OpponentDisconnectedBackButton;
    }

    private void OnClaimClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MatchEnd, ScreenArgument.VictoryOutcome);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
