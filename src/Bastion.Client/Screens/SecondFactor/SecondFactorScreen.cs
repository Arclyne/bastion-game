using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.SecondFactor;

// CU-01 FA-13, FA-14. Login does not branch on the second factor setting yet,
// so nothing reaches this screen today.
public sealed class SecondFactorScreen : FormScreen
{
    private const int CodeLength = 6;
    private const float CodeLifetimeSeconds = 300f;
    private const float NotStarted = -1f;
    private const string ClockFormat = @"m\:ss";

    private const int HintHeight = 20;
    private const int ResendGap = 10;
    private const int ResendButtonTop = HintHeight + ResendGap;
    private const int FieldTop = ResendButtonTop + Theme.SmallButtonHeight + LabelSpace;
    private const int ExpiresGap = 10;
    private const int ExpiresHeight = 20;
    private const int ContentHeight = FieldTop + Theme.FieldHeight + ExpiresGap + ExpiresHeight;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _hint;
    private readonly Button _resendButton;
    private readonly TextField _codeField;
    private readonly TextLine _expiresLine;
    private readonly Button _verifyButton;
    private readonly Button _cancelButton;
    private float _startedAt = NotStarted;

    public SecondFactorScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _hint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(ContentX, top, ContentWidth, HintHeight)
        };

        int resendX = ContentX + ((ContentWidth - Theme.PanelButtonWidth) / 2);
        _resendButton = CreateOutlineButton(
            new Rectangle(resendX, top + ResendButtonTop, Theme.PanelButtonWidth, Theme.SmallButtonHeight));
        _resendButton.Clicked += OnResendClicked;

        _codeField = new TextField
        {
            MaxLength = CodeLength,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, top + FieldTop, ContentWidth, Theme.FieldHeight)
        };

        int expiresTop = top + FieldTop + Theme.FieldHeight + ExpiresGap;
        _expiresLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            Bounds = new Rectangle(ContentX, expiresTop, ContentWidth, ExpiresHeight)
        };

        _verifyButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _verifyButton.Clicked += OnVerifyClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_hint);
        Register(_resendButton);
        RegisterField(_codeField);
        Register(_expiresLine);
        Register(_verifyButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_startedAt < 0f)
        {
            _startedAt = input.ElapsedSeconds;
        }

        float secondsLeft = MathF.Max(0f, CodeLifetimeSeconds - (input.ElapsedSeconds - _startedAt));
        string clock = TimeSpan.FromSeconds(secondsLeft).ToString(ClockFormat, CultureInfo.CurrentCulture);
        _expiresLine.Text = string.Format(CultureInfo.CurrentCulture, TextCatalog.SecondFactorExpiresFormat, clock);
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.SecondFactorSubtitle;
    }

    protected override void ApplyTexts()
    {
        _hint.Text = TextCatalog.SecondFactorHint;
        _resendButton.Title = TextCatalog.SecondFactorResendButton;
        _codeField.Label = TextCatalog.SecondFactorCodeLabel;
        _codeField.Placeholder = TextCatalog.SecondFactorCodePlaceholder;
        _verifyButton.Title = TextCatalog.SecondFactorVerifyButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private void OnVerifyClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.MainMenu);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void OnResendClicked(object? sender, EventArgs e)
    {
        _startedAt = NotStarted;
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.SecondFactorResentBody);
    }
}
