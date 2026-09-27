using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.ReportReview;

// CU-43 main flow step 5. The reason, the description, the match chat with the
// reported messages standing out, and the three ways to close the report.
public sealed class ReportReviewScreen : FormScreen
{
    private const int CardHeight = 384;
    private const int ReasonHeight = 62;
    private const int SectionGap = 18;
    private const int MessageHeight = 46;
    private const int MessageGap = 8;
    private const int VisibleMessages = 3;

    private readonly ValueBox _reasonBox;
    private readonly NoticeBox _description;
    private readonly List<DataRow> _messages = [];
    private readonly Button _sanctionButton;
    private readonly Button _dismissButton;
    private readonly Button _backButton;

    public ReportReviewScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _reasonBox = new ValueBox
        {
            Bounds = new Rectangle(ContentX, top + LabelSpace, ContentWidth, Theme.FieldHeight)
        };

        _description = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, _reasonBox.Bounds.Bottom + SectionGap, ContentWidth, ReasonHeight)
        };

        int messagesTop = _description.Bounds.Bottom + SectionGap;

        // The chat rows stay hidden until the server sends the report.
        for (int messageIndex = 0; messageIndex < VisibleMessages; messageIndex++)
        {
            var message = new DataRow
            {
                IsVisible = false,
                Bounds = new Rectangle(
                    ContentX,
                    messagesTop + (messageIndex * (MessageHeight + MessageGap)),
                    ContentWidth,
                    MessageHeight)
            };

            _messages.Add(message);
            Register(message);
        }

        _sanctionButton = CreatePrimaryButton(false);
        _dismissButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_sanctionButton, _dismissButton, _backButton]);

        _sanctionButton.Clicked += OnSanctionClicked;
        _dismissButton.Clicked += OnDismissClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_reasonBox);
        Register(_description);
        Register(_sanctionButton);
        Register(_dismissButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ReportReviewSubtitle;
    }

    protected override void ApplyTexts()
    {
        _reasonBox.Label = TextCatalog.ReportReviewReasonLabel;
        _sanctionButton.Title = TextCatalog.ReportReviewSanctionButton;
        _dismissButton.Title = TextCatalog.ReportReviewDismissButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnSanctionClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ApplySanction);
    }

    private void OnDismissClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
