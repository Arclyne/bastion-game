using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;

namespace Bastion.Client.Screens;

// Modal shared by MessageErrorScreen, MessageWarningScreen, MessageConfirmScreen and
// MessageSuccessScreen. Only the tone and the labels change between them.
public sealed class MessageDialog
{
    private const int Padding = Theme.CardPadding;
    private const int TitleGap = 20;
    private const int ToneRuleWidth = 44;
    private const int ToneRuleGap = 18;
    private const int DetailGap = 10;
    private const int ButtonGap = 28;
    private const int ButtonSpacing = 12;
    private const int MinimumButtonWidth = 132;
    private const int ButtonTextPadding = 44;
    private const float BackdropOpacity = 0.72f;

    private readonly Button _primaryButton;
    private readonly Button _secondaryButton;

    public MessageDialog(DialogTone tone)
    {
        Tone = tone;

        _primaryButton = new Button { Style = ButtonStyle.Primary, Accent = GetPrimaryColor(tone) };
        _secondaryButton = new Button { Style = ButtonStyle.Secondary };
        _primaryButton.Clicked += OnPrimaryClicked;
        _secondaryButton.Clicked += OnSecondaryClicked;
    }

    public DialogTone Tone { get; }

    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    // Extra line under the body, such as the incident identifier that the
    // server returns with SERVER_ERROR (CU-02 EX-04).
    public string Detail { get; set; } = string.Empty;

    public string PrimaryLabel { get; set; } = string.Empty;

    public string SecondaryLabel { get; set; } = string.Empty;

    public bool HasSecondaryOption => !string.IsNullOrEmpty(SecondaryLabel);

    public bool HasDetail => !string.IsNullOrEmpty(Detail);

    public event EventHandler? PrimaryChosen;

    public event EventHandler? SecondaryChosen;

    // Only success departs from the accent: an orange confirmation button is
    // right for a destructive primary action, a green one would not be.
    public static Color GetPrimaryColor(DialogTone tone)
    {
        return tone == DialogTone.Success ? Theme.Positive : Theme.Accent;
    }

    // Draw lays the buttons out, because measuring a label needs the fonts.
    // On the first frame their bounds are empty, which cannot swallow a click.
    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        _primaryButton.Update(input);

        if (HasSecondaryOption)
        {
            _secondaryButton.Update(input);
        }
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        var backdrop = new Rectangle(0, 0, Theme.WindowWidth, Theme.WindowHeight);
        canvas.Shapes.DrawRectangle(backdrop, Theme.Backdrop * BackdropOpacity);

        DialogMetrics metrics = Measure(canvas);
        Rectangle card = GetCard(metrics);

        canvas.Shapes.DrawRoundedRectangle(card, Theme.CardCornerRadius, Theme.Card);
        DrawToneRule(canvas, card);
        DrawTexts(canvas, card, metrics);
        DrawButtons(canvas, card);
    }

    public Color GetToneColor()
    {
        if (Tone == DialogTone.Success)
        {
            return Theme.Positive;
        }

        if (Tone == DialogTone.Confirm)
        {
            return Theme.Label;
        }

        return Theme.Accent;
    }

    private void OnPrimaryClicked(object? sender, EventArgs e)
    {
        PrimaryChosen?.Invoke(this, EventArgs.Empty);
    }

    private void OnSecondaryClicked(object? sender, EventArgs e)
    {
        SecondaryChosen?.Invoke(this, EventArgs.Empty);
    }

    private static int GetContentWidth()
    {
        return Theme.DialogWidth - (Padding * 2);
    }

    // Measured once per frame. Both the card height and the text positions read
    // from this, so the vertical layout exists in one place only.
    private DialogMetrics Measure(Canvas canvas)
    {
        TextStyle titleStyle = TextStyleFactory.CreateLabel(canvas.Fonts, GetToneColor());
        TextStyle bodyStyle = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        TextStyle detailStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);

        float titleHeight = canvas.Text.GetLineHeight(titleStyle);
        float bodyHeight = canvas.Text.MeasureWrapped(Body, GetContentWidth(), bodyStyle);
        float detailHeight = HasDetail ? canvas.Text.MeasureWrapped(Detail, GetContentWidth(), detailStyle) : 0f;

        float total = Padding + Theme.DialogToneBarHeight + ToneRuleGap;
        total += titleHeight + TitleGap + bodyHeight;
        total += HasDetail ? DetailGap + detailHeight : 0f;
        total += ButtonGap + Theme.DialogButtonHeight + Padding;

        return new DialogMetrics
        {
            TitleStyle = titleStyle,
            BodyStyle = bodyStyle,
            DetailStyle = detailStyle,
            TitleHeight = titleHeight,
            BodyHeight = bodyHeight,
            DetailHeight = detailHeight,
            TotalHeight = total
        };
    }

    private static Rectangle GetCard(DialogMetrics metrics)
    {
        int height = (int)MathF.Round(metrics.TotalHeight);
        int x = (Theme.WindowWidth - Theme.DialogWidth) / 2;
        int y = (Theme.WindowHeight - height) / 2;

        return new Rectangle(x, y, Theme.DialogWidth, height);
    }

    // A short rule above the title instead of a full width bar on the top edge:
    // there the rounded corners cut it and its ends hang outside the card.
    private void DrawToneRule(Canvas canvas, Rectangle card)
    {
        var rule = new Rectangle(card.X + Padding, card.Y + Padding, ToneRuleWidth, Theme.DialogToneBarHeight);
        canvas.Shapes.DrawRoundedRectangle(rule, Theme.DialogToneBarHeight / 2, GetToneColor());
    }

    private void DrawTexts(Canvas canvas, Rectangle card, DialogMetrics metrics)
    {
        float y = card.Y + Padding + Theme.DialogToneBarHeight + ToneRuleGap;
        canvas.Text.Draw(Title, new Vector2(card.X + Padding, MathF.Round(y)), metrics.TitleStyle);

        y += metrics.TitleHeight + TitleGap;

        var bodyArea = new Rectangle(card.X + Padding, (int)MathF.Round(y), GetContentWidth(), card.Height);
        canvas.Text.DrawWrapped(Body, bodyArea, metrics.BodyStyle);

        if (!HasDetail)
        {
            return;
        }

        y += metrics.BodyHeight + DetailGap;
        var detailArea = new Rectangle(card.X + Padding, (int)MathF.Round(y), GetContentWidth(), card.Height);
        canvas.Text.DrawWrapped(Detail, detailArea, metrics.DetailStyle);
    }

    private void DrawButtons(Canvas canvas, Rectangle card)
    {
        int top = card.Bottom - Padding - Theme.DialogButtonHeight;
        int primaryWidth = GetButtonWidth(canvas, PrimaryLabel);
        int primaryX = card.Right - Padding - primaryWidth;

        _primaryButton.Title = PrimaryLabel;
        _primaryButton.MoveTo(new Rectangle(primaryX, top, primaryWidth, Theme.DialogButtonHeight));
        _primaryButton.Draw(canvas);

        if (!HasSecondaryOption)
        {
            return;
        }

        int secondaryWidth = GetButtonWidth(canvas, SecondaryLabel);
        int secondaryX = primaryX - ButtonSpacing - secondaryWidth;

        _secondaryButton.Title = SecondaryLabel;
        _secondaryButton.MoveTo(new Rectangle(secondaryX, top, secondaryWidth, Theme.DialogButtonHeight));
        _secondaryButton.Draw(canvas);
    }

    private static int GetButtonWidth(Canvas canvas, string label)
    {
        TextStyle style = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextLight);
        float width = canvas.Text.Measure(label, style) + ButtonTextPadding;

        return (int)MathF.Max(width, MinimumButtonWidth);
    }
}
