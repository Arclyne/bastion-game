using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens;

// Chrome, focus order and button geometry shared by every account screen.
public abstract class FormScreen : IScreen
{
    protected const int NarrowCardWidth = 560;
    protected const int WideCardWidth = 900;
    protected const int PanelNarrowWidth = 640;
    protected const int RowSpacing = 98;
    protected const int LabelSpace = 22;
    protected const int Gutter = 32;
    protected const int ActionGap = 12;

    private const int AlternateEvery = 2;
    private const int ButtonGap = 22;
    private const int ButtonSpacing = 12;
    private const int BackLinkWidth = 200;

    private readonly List<Control> _controls = [];
    private readonly List<TextField> _focusableFields = [];
    private readonly DropDown? _languagePicker;
    private readonly Button? _backLink;
    private readonly ScreenLayout _layout;

    protected FormScreen(INavigator navigator, int cardWidth, int cardHeight)
        : this(navigator, new CardShape
        {
            Width = cardWidth,
            Height = cardHeight,
            Layout = ScreenLayout.Chrome
        })
    {
    }

    protected FormScreen(INavigator navigator, CardShape card)
    {
        ArgumentNullException.ThrowIfNull(navigator);
        ArgumentNullException.ThrowIfNull(card);

        Navigator = navigator;
        _layout = card.Layout;

        int x = (Theme.WindowWidth - card.Width) / 2;
        int top = card.Layout == ScreenLayout.Chrome ? ScreenChrome.CardTop : Theme.PanelTop;
        Card = new Rectangle(x, top, card.Width, card.Height);

        if (card.Layout == ScreenLayout.Chrome)
        {
            int primaryTop = Card.Bottom + ButtonGap;
            PrimaryButtonBounds = new Rectangle(x, primaryTop, card.Width, Theme.PrimaryButtonHeight);

            int secondaryTop = primaryTop + Theme.PrimaryButtonHeight + ButtonSpacing;
            SecondaryButtonBounds = new Rectangle(x, secondaryTop, card.Width, Theme.SecondaryButtonHeight);

            _languagePicker = LanguagePicker.Create();
            _languagePicker.SelectionChanged += OnLanguageSelected;
            return;
        }

        int buttonTop = Card.Bottom - Theme.CardPadding - Theme.PanelButtonHeight;
        PrimaryButtonBounds = new Rectangle(
            Card.Right - Theme.CardPadding - Theme.PanelButtonWidth,
            buttonTop,
            Theme.PanelButtonWidth,
            Theme.PanelButtonHeight);
        SecondaryButtonBounds = new Rectangle(
            Card.X + Theme.CardPadding,
            buttonTop,
            Theme.PanelButtonWidth,
            Theme.PanelButtonHeight);

        _backLink = new Button
        {
            Style = ButtonStyle.Link,
            Bounds = new Rectangle(Theme.PanelMargin, Theme.PanelMargin, BackLinkWidth, Theme.PanelBackHeight)
        };
        _backLink.Clicked += OnBackLinkClicked;
    }

    protected INavigator Navigator { get; }

    protected Rectangle Card { get; }

    protected Rectangle PrimaryButtonBounds { get; }

    protected Rectangle SecondaryButtonBounds { get; }

    protected int ContentX => Card.X + Theme.CardPadding;

    protected int ContentWidth => Card.Width - (Theme.CardPadding * 2);

    protected int ContentTop => _layout == ScreenLayout.Chrome ? Card.Y + Theme.CardPadding : PanelContentTop;

    protected int FirstRowTop => ContentTop + LabelSpace;

    protected int PanelTitleTop => Card.Y + Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap;

    protected int PanelContentTop => _layout == ScreenLayout.Panel
        ? PanelTitleTop + Theme.PanelTitleHeight + Theme.PanelGap
        : PanelTitleTop;

    protected virtual int TitleLeftInset => 0;

    protected int ColumnWidth => (ContentWidth - Gutter) / 2;

    protected virtual int RowPitch => RowSpacing;

    public virtual void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (_languagePicker is not null)
        {
            bool isPickerAlreadyOpen = _languagePicker.IsOpen;
            _languagePicker.Update(input);

            // The click that closes the open list must not also reach the control below it.
            if (isPickerAlreadyOpen && input.HasClicked)
            {
                return;
            }
        }

        _backLink?.Update(input);

        if (input.HasClicked)
        {
            ResolveFocus(input);
        }

        if (input.IsKeyNewlyPressed(Keys.Tab))
        {
            MoveFocus(IsShiftPressed(input));
        }

        foreach (Control control in _controls)
        {
            control.Update(input);
        }
    }

    public virtual void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (_layout == ScreenLayout.Chrome)
        {
            ScreenChrome.Draw(canvas, GetSubtitle());
            canvas.Shapes.DrawRoundedRectangle(Card, Theme.CardCornerRadius, Theme.Card);
        }
        else
        {
            canvas.Shapes.DrawRectangle(new Rectangle(0, 0, Theme.WindowWidth, Theme.WindowHeight), Theme.Card);
        }

        if (_backLink is not null)
        {
            _backLink.Title = GetBackLabel();
            _backLink.Draw(canvas);
        }

        if (_layout == ScreenLayout.Panel)
        {
            TextStyle heading = TextStyleFactory.CreateHeading(canvas.Fonts, Theme.TextDark);
            canvas.Text.Draw(GetSubtitle(), new Vector2(ContentX + TitleLeftInset, PanelTitleTop), heading);
        }

        foreach (Control control in _controls)
        {
            control.Draw(canvas);
        }

        _languagePicker?.Draw(canvas);
    }

    // The height of a card filled with a list of rows. Several screens work it
    // out the same way, and some of them add their own header on top.
    protected static int ComputeListCardHeight(int visibleRows, int rowHeight, int rowGap)
    {
        return (Theme.CardPadding * 2) + (visibleRows * rowHeight) + ((visibleRows - 1) * rowGap);
    }

    protected static ValueBox CreateValueBox(Rectangle bounds)
    {
        return new ValueBox { Bounds = bounds };
    }

    // Lists that alternate the style of their rows share this, so the modulus
    // is not repeated in every screen.
    protected static bool IsEvenRow(int index)
    {
        return index % AlternateEvery == 0;
    }

    protected virtual string GetBackLabel()
    {
        return TextCatalog.CommonBackLink;
    }

    protected abstract string GetSubtitle();

    protected abstract void ApplyTexts();

    // Draw order is registration order, so anything that can overlap the rest
    // is registered last.
    protected void Register(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        _controls.Add(control);
    }

    protected void RegisterField(TextField field)
    {
        ArgumentNullException.ThrowIfNull(field);

        _controls.Add(field);
        _focusableFields.Add(field);
    }

    protected Button CreatePrimaryButton(bool hasArrow)
    {
        return new Button
        {
            Style = ButtonStyle.Primary,
            HasArrow = hasArrow,
            Bounds = PrimaryButtonBounds
        };
    }

    protected Button CreateSecondaryButton()
    {
        return new Button
        {
            Style = ButtonStyle.Secondary,
            Bounds = SecondaryButtonBounds
        };
    }

    protected Button CreateOutlineButton(Rectangle bounds)
    {
        return new Button { Style = ButtonStyle.Outline, Bounds = bounds };
    }

    // Stacked buttons fall outside the window on the taller cards, so those
    // screens lay their actions out in one row.
    protected void LayOutActionsInRow(IReadOnlyList<Button> actions)
    {
        ArgumentNullException.ThrowIfNull(actions);

        int gaps = ActionGap * (actions.Count - 1);
        int width = (PrimaryButtonBounds.Width - gaps) / actions.Count;

        for (int actionIndex = 0; actionIndex < actions.Count; actionIndex++)
        {
            int x = PrimaryButtonBounds.X + (actionIndex * (width + ActionGap));
            actions[actionIndex].MoveTo(new Rectangle(x, PrimaryButtonBounds.Y, width, Theme.PrimaryButtonHeight));
        }
    }

    protected void FocusFirstField()
    {
        if (_focusableFields.Count > 0)
        {
            _focusableFields[0].IsFocused = true;
        }
    }

    protected Rectangle GetRow(int index)
    {
        return new Rectangle(ContentX, FirstRowTop + (index * RowPitch), ContentWidth, Theme.FieldHeight);
    }

    protected Rectangle GetCell(int row, bool isRightColumn)
    {
        int x = isRightColumn ? ContentX + ColumnWidth + Gutter : ContentX;

        return new Rectangle(x, FirstRowTop + (row * RowPitch), ColumnWidth, Theme.FieldHeight);
    }

    private static bool IsFieldFocused(TextField field)
    {
        return field.IsFocused;
    }

    private static bool IsShiftPressed(InputState input)
    {
        return input.IsKeyPressed(Keys.LeftShift) || input.IsKeyPressed(Keys.RightShift);
    }

    private void OnBackLinkClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void OnLanguageSelected(object? sender, SelectionChangedEventArgs e)
    {
        // Only a screen that draws the picker subscribes to it, so this guard
        // states the invariant instead of silencing the nullable warning.
        if (_languagePicker is null)
        {
            return;
        }

        LanguagePicker.Apply(e.SelectedIndex);
        _languagePicker.Options = LanguagePicker.GetNames();
        ApplyTexts();
    }

    private void ResolveFocus(InputState input)
    {
        foreach (TextField field in _focusableFields)
        {
            field.IsFocused = field.Bounds.Contains(input.MousePosition);
        }
    }

    private void MoveFocus(bool isBackwards)
    {
        if (_focusableFields.Count == 0)
        {
            return;
        }

        int current = _focusableFields.FindIndex(IsFieldFocused);
        int step = isBackwards ? -1 : 1;
        int next = current < 0 ? 0 : (current + step + _focusableFields.Count) % _focusableFields.Count;

        for (int fieldIndex = 0; fieldIndex < _focusableFields.Count; fieldIndex++)
        {
            _focusableFields[fieldIndex].IsFocused = fieldIndex == next;
        }
    }
}
