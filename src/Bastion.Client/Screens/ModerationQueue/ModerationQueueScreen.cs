using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.ModerationQueue;

// CU-43 main flow step 3. Reports and appeals waiting, with their reason, their
// age and how many reports the reported player already carries.
public sealed class ModerationQueueScreen : FormScreen
{
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 4;

    private static readonly int _cardHeight = ComputeListCardHeight(VisibleRows, RowHeight, RowGap);

    private readonly List<DataRow> _rows = [];
    private readonly TextBlock _emptyNotice;
    private readonly Button _reviewButton;
    private readonly Button _backButton;

    public ModerationQueueScreen(INavigator navigator)
        : base(navigator, WideCardWidth, _cardHeight)
    {
        // The rows stay hidden until the server sends the queue.
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

        _reviewButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _reviewButton.Clicked += OnReviewClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_emptyNotice);
        Register(_reviewButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ModerationQueueSubtitle;
    }

    protected override void ApplyTexts()
    {
        _emptyNotice.Text = TextCatalog.ModerationQueueEmpty;
        _reviewButton.Title = TextCatalog.ModerationQueueReviewButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnReviewClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ReportReview);
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
