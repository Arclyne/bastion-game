using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bastion.Client.Controls;

public sealed class TextField : Control
{
    private const char PasswordBullet = '•';
    private const int HorizontalPadding = 16;
    private const int LabelOffset = 22;
    private const int CaretWidth = 2;
    private const int CaretGap = 2;
    private const float BlinksPerSecond = 2f;
    private const int DefaultMaxLength = 64;
    private const int BlinkPhases = 2;
    private const int CaretVerticalInset = 2;
    private const int CaretOffset = 1;
    private const int WarningInset = 2;
    private const float FocusBorderOpacity = 0.55f;

    private const char Backspace = '\b';

    private readonly StringBuilder _text = new();
    private float _elapsedSeconds;

    public string Label { get; set; } = string.Empty;

    public string Placeholder { get; set; } = string.Empty;

    public bool IsPassword { get; init; }

    public int MaxLength { get; init; } = DefaultMaxLength;

    public bool IsCentered { get; init; }

    public string Text => _text.ToString();

    public void SetText(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _text.Clear();
        _text.Append(value.Length <= MaxLength ? value : value[..MaxLength]);
    }

    public override void Update(InputState input)
    {
        base.Update(input);
        _elapsedSeconds = input.ElapsedSeconds;

        if (!IsEnabled || !IsVisible || !IsFocused)
        {
            return;
        }

        AppendTypedCharacters(input);

        if (input.IsKeyNewlyPressed(Keys.Delete) && _text.Length > 0)
        {
            _text.Length--;
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        DrawLabel(canvas);
        DrawBox(canvas);
        DrawContent(canvas);
        DrawWarning(canvas);
    }

    private void AppendTypedCharacters(InputState input)
    {
        foreach (char character in input.Characters)
        {
            if (character == Backspace)
            {
                RemoveLastCharacter();
                continue;
            }

            if (!char.IsControl(character) && _text.Length < MaxLength)
            {
                _text.Append(character);
            }
        }
    }

    private void RemoveLastCharacter()
    {
        if (_text.Length > 0)
        {
            _text.Length--;
        }
    }

    private void DrawLabel(Canvas canvas)
    {
        if (string.IsNullOrEmpty(Label))
        {
            return;
        }

        Color color = HasWarning ? Theme.Accent : Theme.Label;
        TextStyle style = TextStyleFactory.CreateLabel(canvas.Fonts, color);
        canvas.Text.Draw(Label, new Vector2(Bounds.X, Bounds.Y - LabelOffset), style);
    }

    private void DrawBox(Canvas canvas)
    {
        Color fill = IsFocused ? Theme.FieldFocused : Theme.Field;
        canvas.Shapes.DrawRoundedRectangle(Bounds, Theme.FieldCornerRadius, fill);

        if (HasWarning)
        {
            canvas.Shapes.DrawRoundedBorder(
                Bounds,
                BorderStyleFactory.CreateThick(Theme.FieldCornerRadius, Theme.Accent));
            return;
        }

        if (IsFocused)
        {
            canvas.Shapes.DrawRoundedBorder(
                Bounds,
                BorderStyleFactory.CreateThick(Theme.FieldCornerRadius, Theme.Accent * FocusBorderOpacity));
        }
    }

    private void DrawContent(Canvas canvas)
    {
        string visible = IsPassword ? new string(PasswordBullet, _text.Length) : Text;
        bool isEmpty = visible.Length == 0;

        string shown = isEmpty ? HidePlaceholderWhenFocused() : visible;
        Color color = isEmpty ? Theme.Placeholder : Theme.TextDark;
        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, color);

        var area = new Rectangle(
            Bounds.X + HorizontalPadding,
            Bounds.Y,
            Bounds.Width - (HorizontalPadding * 2),
            Bounds.Height);

        shown = canvas.Text.FitEnd(shown, area.Width - CaretWidth - CaretGap, style);

        float width = canvas.Text.Measure(shown, style);
        float x = IsCentered ? area.X + ((area.Width - width) / 2f) : area.X;
        float y = area.Y + ((area.Height - canvas.Text.GetLineHeight(style)) / 2f);

        canvas.Text.Draw(shown, new Vector2(MathF.Round(x), MathF.Round(y)), style);
        DrawCaret(canvas, new Vector2(isEmpty ? x : x + width + CaretOffset, y), style);
    }

    private string HidePlaceholderWhenFocused()
    {
        return IsFocused ? string.Empty : Placeholder;
    }

    private void DrawCaret(Canvas canvas, Vector2 position, TextStyle style)
    {
        if (!IsFocused || (int)(_elapsedSeconds * BlinksPerSecond) % BlinkPhases != 0)
        {
            return;
        }

        int height = (int)canvas.Text.GetLineHeight(style) - (CaretVerticalInset * 2);
        var caret = new Rectangle((int)position.X, (int)position.Y + CaretVerticalInset, CaretWidth, height);
        canvas.Shapes.DrawRectangle(caret, Theme.TextDark);
    }

    private void DrawWarning(Canvas canvas)
    {
        if (!HasWarning)
        {
            return;
        }

        TextStyle style = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Accent);
        var position = new Vector2(Bounds.X + WarningInset, Bounds.Bottom + Theme.WarningOffset);
        canvas.Text.Draw(Warning ?? string.Empty, position, style);
    }
}
