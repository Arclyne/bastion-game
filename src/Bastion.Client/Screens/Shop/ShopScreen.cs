using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Shop;

public sealed class ShopScreen : FormScreen
{
    private const int CardHeight = 400;
    private const int BalanceWidth = 180;
    private const int BalanceHeight = 68;
    private const int SectionGap = 20;
    private const int TileColumns = 4;
    private const int TileRows = 2;
    private const int TileHeight = 132;
    private const int TileGap = 14;

    private readonly StatTile _balance;
    private readonly List<AvatarBox> _items = [];
    private readonly TextBlock _emptyNotice;
    private readonly Button _buyButton;
    private readonly Button _boxButton;
    private readonly Button _backButton;

    public ShopScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _balance = new StatTile
        {
            Bounds = new Rectangle(Card.Right - Theme.CardPadding - BalanceWidth, top, BalanceWidth, BalanceHeight)
        };

        int gridTop = top + BalanceHeight + SectionGap;

        for (int itemIndex = 0; itemIndex < TileColumns * TileRows; itemIndex++)
        {
            var item = new AvatarBox
            {
                IsVisible = false,
                Bounds = GetTileBounds(itemIndex, gridTop)
            };

            _items.Add(item);
            Register(item);
        }

        _emptyNotice = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, gridTop, ContentWidth, TileHeight)
        };

        _buyButton = CreatePrimaryButton(true);
        _boxButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_buyButton, _boxButton, _backButton]);
        _buyButton.Clicked += OnBuyClicked;
        _boxButton.Clicked += OnBoxClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_balance);
        Register(_emptyNotice);
        Register(_buyButton);
        Register(_boxButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ShopSubtitle;
    }

    protected override void ApplyTexts()
    {
        _balance.Caption = TextCatalog.CoinHistoryBalanceCaption;
        _emptyNotice.Text = TextCatalog.ShopEmpty;
        _buyButton.Title = TextCatalog.ShopBuyButton;
        _boxButton.Title = TextCatalog.ShopBoxButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnBuyClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.PurchaseConfirm);
    }

    private void OnBoxClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.BoxPurchaseConfirm);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private int GetTileWidth()
    {
        return (ContentWidth - (TileGap * (TileColumns - 1))) / TileColumns;
    }

    private Rectangle GetTileBounds(int index, int top)
    {
        int width = GetTileWidth();
        int column = index % TileColumns;
        int row = index / TileColumns;

        return new Rectangle(
            ContentX + (column * (width + TileGap)),
            top + (row * (TileHeight + TileGap)),
            width,
            TileHeight);
    }
}
