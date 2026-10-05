using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.BoxPurchaseConfirm;

public sealed class BoxPurchaseConfirmScreen : FormScreen
{
    private const int CardHeight = 330;
    private const int SectionGap = 20;
    private const int NoticeHeight = 76;
    private const int ColumnCount = 2;

    private readonly ValueBox _priceBox;
    private readonly ValueBox _remainingBox;
    private readonly NoticeBox _guaranteeNotice;
    private readonly Button _confirmButton;
    private readonly Button _cancelButton;

    public BoxPurchaseConfirmScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / ColumnCount;

        _priceBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight));
        _remainingBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop, column, Theme.FieldHeight));

        _guaranteeNotice = new NoticeBox
        {
            IsVisible = false,
            Bounds = new Rectangle(
                ContentX,
                FirstRowTop + Theme.FieldHeight + SectionGap,
                ContentWidth,
                NoticeHeight)
        };

        _confirmButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _confirmButton.Clicked += OnConfirmClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_priceBox);
        Register(_remainingBox);
        Register(_guaranteeNotice);
        Register(_confirmButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.BoxPurchaseSubtitle;
    }

    protected override void ApplyTexts()
    {
        _priceBox.Label = TextCatalog.PurchaseConfirmPriceLabel;
        _remainingBox.Label = TextCatalog.PurchaseConfirmRemainingLabel;
        _confirmButton.Title = TextCatalog.BoxPurchaseConfirmButton;
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
