using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Appeal;

public sealed class AppealScreen : FormScreen
{
    private const int CardHeight = 400;
    private const int DetailHeight = 92;
    private const int SectionGap = 22;
    private const int NoticeHeight = 62;
    private const int MaxAppealLength = 500;

    private readonly NoticeBox _detail;
    private readonly TextField _appealField;
    private readonly NoticeBox _stillActiveNotice;
    private readonly Button _sendButton;
    private readonly Button _cancelButton;

    public AppealScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _detail = new NoticeBox
        {
            Bounds = new Rectangle(ContentX, Card.Y + Theme.CardPadding, ContentWidth, DetailHeight)
        };

        int appealTop = _detail.Bounds.Bottom + SectionGap + LabelSpace;
        _appealField = new TextField
        {
            MaxLength = MaxAppealLength,
            Bounds = new Rectangle(ContentX, appealTop, ContentWidth, Theme.FieldHeight)
        };

        _stillActiveNotice = new NoticeBox
        {
            IsCritical = true,
            Bounds = new Rectangle(ContentX, _appealField.Bounds.Bottom + SectionGap, ContentWidth, NoticeHeight)
        };

        _sendButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        _sendButton.Clicked += OnSendClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_detail);
        RegisterField(_appealField);
        Register(_stillActiveNotice);
        Register(_sendButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AppealSubtitle;
    }

    protected override void ApplyTexts()
    {
        _appealField.Label = TextCatalog.AppealTextLabel;
        _appealField.Placeholder = TextCatalog.AppealTextPlaceholder;
        _stillActiveNotice.Text = TextCatalog.AppealStillActiveNotice;
        _sendButton.Title = TextCatalog.AppealSendButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
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
