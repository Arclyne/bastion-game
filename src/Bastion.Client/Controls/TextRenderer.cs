using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Controls;

public sealed class TextRenderer
{
    private const char WordSeparator = ' ';

    private readonly SpriteBatch _batch;

    public TextRenderer(SpriteBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        _batch = batch;
    }

    public void Draw(string text, Vector2 position, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (style.Tracking == 0f)
        {
            DrawRun(text, position, style);
            return;
        }

        float x = position.X;

        foreach (char character in text)
        {
            string letter = character.ToString();
            DrawRun(letter, new Vector2(x, position.Y), style);
            x += (style.Font.MeasureString(letter).X * style.Scale) + style.Tracking;
        }
    }

    public void DrawCentered(string text, Rectangle area, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        float width = Measure(text, style);
        float x = area.X + ((area.Width - width) / 2f);
        float y = area.Y + ((area.Height - GetLineHeight(style)) / 2f);

        Draw(text, new Vector2(MathF.Round(x), MathF.Round(y)), style);
    }

    public void DrawWrapped(string text, Rectangle area, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        float lineHeight = GetLineHeight(style);
        float y = area.Y;

        foreach (string line in SplitIntoLines(text, area.Width, style))
        {
            Draw(line, new Vector2(area.X, MathF.Round(y)), style);
            y += lineHeight;
        }
    }

    public float Measure(string text, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        if (string.IsNullOrEmpty(text))
        {
            return 0f;
        }

        if (style.Tracking == 0f)
        {
            return style.Font.MeasureString(text).X * style.Scale;
        }

        float width = 0f;

        foreach (char character in text)
        {
            width += (style.Font.MeasureString(character.ToString()).X * style.Scale) + style.Tracking;
        }

        return width - style.Tracking;
    }

    // The longest ending of the text that fits in the width. Editing only
    // happens at the end of a field, so the end is what has to stay visible
    // when the value outgrows its box.
    public string FitEnd(string text, float width, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        if (string.IsNullOrEmpty(text) || Measure(text, style) <= width)
        {
            return text;
        }

        // Dropping a leading character never widens the run, so the first
        // start that fits is found by halving instead of one character at a
        // time, which would measure the whole string on every frame.
        int low = 1;
        int high = text.Length;

        while (low < high)
        {
            int middle = low + ((high - low) / 2);

            if (Measure(text[middle..], style) <= width)
            {
                high = middle;
            }
            else
            {
                low = middle + 1;
            }
        }

        return text[low..];
    }

    public float MeasureWrapped(string text, int width, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        return SplitIntoLines(text, width, style).Count * GetLineHeight(style);
    }

    public float GetLineHeight(TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        return style.Font.LineSpacing * style.Scale;
    }

    // Greedy word wrap. A word longer than the available width gets its own
    // line and overflows, which is preferable to splitting it mid word.
    public IReadOnlyList<string> SplitIntoLines(string text, int width, TextStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        var lines = new List<string>();

        if (string.IsNullOrEmpty(text))
        {
            return lines;
        }

        string current = string.Empty;

        foreach (string word in text.Split(WordSeparator))
        {
            string candidate = current.Length == 0 ? word : current + WordSeparator + word;

            if (current.Length > 0 && Measure(candidate, style) > width)
            {
                lines.Add(current);
                current = word;
                continue;
            }

            current = candidate;
        }

        lines.Add(current);

        return lines;
    }

    private void DrawRun(string text, Vector2 position, TextStyle style)
    {
        _batch.DrawString(
            style.Font,
            text,
            position,
            style.Color,
            0f,
            Vector2.Zero,
            style.Scale,
            SpriteEffects.None,
            0f);
    }
}
