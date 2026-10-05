using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Report;

public sealed class ReportScreen : FormScreen
{
    private const int CardHeight = 470;
    private const int ReasonCount = 5;
    private const int SectionGap = 24;
    private const int NoticeHeight = 62;
    private const int MaxDescriptionLength = 300;

    private readonly OptionList _reasons;
    private readonly TextField _description;
    private readonly NoticeBox _evidenceNotice;
    private readonly Button _sendButton;
    private readonly Button _cancelButton;

    public ReportScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int listTop = Card.Y + Theme.CardPadding;
        _reasons = new OptionList
        {
            Options = BuildReasons(),
            Bounds = new Rectangle(ContentX, listTop, ContentWidth, OptionList.GetRequiredHeight(ReasonCount))
        };

        int descriptionTop = _reasons.Bounds.Bottom + SectionGap + LabelSpace;
        _description = new TextField
        {
            MaxLength = MaxDescriptionLength,
            Bounds = new Rectangle(ContentX, descriptionTop, ContentWidth, Theme.FieldHeight)
        };

        _evidenceNotice = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, _description.Bounds.Bottom + SectionGap, ContentWidth, NoticeHeight)
        };

        _sendButton = CreatePrimaryButton(false);
        _cancelButton = CreateSecondaryButton();
        LayOutActionsInRow([_sendButton, _cancelButton]);
        _sendButton.Clicked += OnSendClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_reasons);
        RegisterField(_description);
        Register(_evidenceNotice);
        Register(_sendButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ReportSubtitle;
    }

    protected override void ApplyTexts()
    {
        _reasons.Options = BuildReasons();
        _description.Label = TextCatalog.ReportDescriptionLabel;
        _description.Placeholder = TextCatalog.ReportDescriptionPlaceholder;
        _evidenceNotice.Text = TextCatalog.ReportEvidenceNotice;
        _sendButton.Title = TextCatalog.ReportSendButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private static IReadOnlyList<string> BuildReasons()
    {
        return
        [
            TextCatalog.ReportReasonCheating,
            TextCatalog.ReportReasonAbuse,
            TextCatalog.ReportReasonName,
            TextCatalog.ReportReasonLeaving,
            TextCatalog.ReportReasonOther
        ];
    }

    private void OnSendClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
