using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class SettingRow : Control
{
    private const int ButtonWidth = 130;
    private const int TextGap = 4;
    private const int DividerInset = 2;
    private const int DividerThickness = 1;

    private readonly Button _button;

    public SettingRow()
    {
        _button = new Button { Style = ButtonStyle.Outline };
        _button.Clicked += OnButtonClicked;
    }

    public string Title { get; set; } = string.Empty;

    public string Subtitle { get; set; } = string.Empty;

    public string ButtonLabel { get; set; } = string.Empty;

    public bool IsButtonEnabled { get; set; } = true;

    public event EventHandler? Clicked;

    public override void Update(InputState input)
    {
        base.Update(input);
        _button.IsEnabled = IsButtonEnabled;
        _button.Update(input);
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        TextStyle titleStyle = TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextDark);
        TextStyle subtitleStyle = TextStyleFactory.CreateSmall(canvas.Fonts, Theme.Placeholder);
        float total = canvas.Text.GetLineHeight(titleStyle) + TextGap + canvas.Text.GetLineHeight(subtitleStyle);
        float y = Bounds.Y + ((Bounds.Height - total) / 2f);
        canvas.Text.Draw(Title, new Vector2(Bounds.X, MathF.Round(y)), titleStyle);
        y += canvas.Text.GetLineHeight(titleStyle) + TextGap;
        canvas.Text.Draw(Subtitle, new Vector2(Bounds.X, MathF.Round(y)), subtitleStyle);

        _button.Title = ButtonLabel;
        _button.IsEnabled = IsButtonEnabled;
        _button.MoveTo(new Rectangle(
            Bounds.Right - ButtonWidth,
            Bounds.Y + ((Bounds.Height - Theme.SmallButtonHeight) / 2),
            ButtonWidth,
            Theme.SmallButtonHeight));
        _button.Draw(canvas);

        Hairline.DrawHorizontal(
            canvas,
            new Rectangle(Bounds.X, Bounds.Bottom - DividerInset, Bounds.Width, DividerThickness),
            Theme.CheckBoxBorder);
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}
