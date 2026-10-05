using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.MatchHistory;

public sealed class MatchHistoryScreen : FormScreen
{
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 5;
    private const int FirstRowIndex = 0;

    private static readonly int _cardHeight = ComputeListCardHeight(VisibleRows, RowHeight, RowGap);

    private readonly List<DataRow> _rows = [];
    private readonly TextLine _emptyLine;
    private readonly Button _backButton;

    public MatchHistoryScreen(INavigator navigator)
        : base(navigator, WideCardWidth, _cardHeight)
    {
        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = GetRowBounds(rowIndex)
            };
            row.Clicked += OnRowClicked;
            _rows.Add(row);
            Register(row);
        }

        _emptyLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = GetRowBounds(FirstRowIndex)
        };
        Register(_emptyLine);

        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.MatchHistorySubtitle;
    }

    protected override void ApplyTexts()
    {
        _emptyLine.Text = TextCatalog.MatchHistoryEmpty;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnRowClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Replay);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
