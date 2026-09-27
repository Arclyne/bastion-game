using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.PurchaseConfirm;

// CU-38 main flow step 4. The price, the balance now and the balance after,
// which CU-38 RN-07 asks to show before charging anything. The values come
// from the server, so the boxes start empty.
public sealed class PurchaseConfirmScreen : FormScreen
{
    private const int CardHeight = 330;
    private const int SectionGap = 20;
    private const int ColumnCount = 2;
    private const int RemainingRow = 2;

    private readonly ValueBox _itemBox;
    private readonly ValueBox _priceBox;
    private readonly ValueBox _balanceBox;
    private readonly ValueBox _remainingBox;
    private readonly Button _confirmButton;
    private readonly Button _cancelButton;

    public PurchaseConfirmScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / ColumnCount;

        _itemBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, ContentWidth, Theme.FieldHeight));
        _priceBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _balanceBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _remainingBox = CreateValueBox(
            new Rectangle(ContentX, FirstRowTop + (RowSpacing * RemainingRow), ContentWidth, Theme.FieldHeight));

        _confirmButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _confirmButton.Clicked += OnConfirmClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_itemBox);
        Register(_priceBox);
        Register(_balanceBox);
        Register(_remainingBox);
        Register(_confirmButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.PurchaseConfirmSubtitle;
    }

    protected override void ApplyTexts()
    {
        _itemBox.Label = TextCatalog.PurchaseConfirmItemLabel;
        _priceBox.Label = TextCatalog.PurchaseConfirmPriceLabel;
        _balanceBox.Label = TextCatalog.PurchaseConfirmBalanceLabel;
        _remainingBox.Label = TextCatalog.PurchaseConfirmRemainingLabel;
        _confirmButton.Title = TextCatalog.PurchaseConfirmButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private void OnConfirmClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.CoinHistory);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
