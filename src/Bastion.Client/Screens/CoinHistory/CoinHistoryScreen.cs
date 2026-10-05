using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.CoinHistory;

public sealed class CoinHistoryScreen : FormScreen
{
    private const int CardHeight = 352;
    private const int BalanceHeight = 76;
    private const int BalanceGap = 18;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;

    private readonly StatTile _balanceTile;
    private readonly List<DataRow> _rows = [];
    private readonly TextBlock _emptyNotice;
    private readonly Button _backButton;

    public CoinHistoryScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _balanceTile = new StatTile
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, BalanceHeight)
        };

        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = GetRowBounds(rowIndex)
            };

            _rows.Add(row);
            Register(row);
        }

        _emptyNotice = new TextBlock
        {
            IsSmall = true,
            Bounds = GetRowBounds(0)
        };

        _backButton = CreateSecondaryButton();
        _backButton.MoveTo(PrimaryButtonBounds);
        _backButton.Clicked += OnBackClicked;

        Register(_balanceTile);
        Register(_emptyNotice);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.CoinHistorySubtitle;
    }

    protected override void ApplyTexts()
    {
        _balanceTile.Caption = TextCatalog.CoinHistoryBalanceCaption;
        _emptyNotice.Text = TextCatalog.CoinHistoryEmpty;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private Rectangle GetRowBounds(int index)
    {
        int top = Card.Y + Theme.CardPadding + BalanceHeight + BalanceGap + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
