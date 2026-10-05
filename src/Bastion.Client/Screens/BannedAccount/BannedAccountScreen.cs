using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.BannedAccount;

public sealed class BannedAccountScreen : FormScreen
{
    private const int NoticeHeight = 40;
    private const int SectionGap = 20;
    private const int RowGap = 16;

    private const int ReasonRowTop = NoticeHeight + SectionGap;
    private const int TypeRowTop = ReasonRowTop + LabelSpace + Theme.FieldHeight + RowGap;
    private const int EndRowTop = TypeRowTop + LabelSpace + Theme.FieldHeight + RowGap;
    private const int ContentHeight = EndRowTop + LabelSpace + Theme.FieldHeight;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextBlock _notice;
    private readonly ValueBox _reasonBox;
    private readonly ValueBox _typeBox;
    private readonly ValueBox _endBox;
    private readonly Button _appealButton;
    private readonly Button _exitButton;

    public BannedAccountScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _notice = new TextBlock { Bounds = new Rectangle(ContentX, top, ContentWidth, NoticeHeight) };
        _reasonBox = CreateValueBox(GetRowBounds(top, ReasonRowTop));
        _typeBox = CreateValueBox(GetRowBounds(top, TypeRowTop));
        _endBox = CreateValueBox(GetRowBounds(top, EndRowTop));

        _appealButton = CreatePrimaryButton(true);
        _exitButton = CreateSecondaryButton();
        _appealButton.Clicked += OnAppealClicked;
        _exitButton.Clicked += OnExitClicked;

        Register(_notice);
        Register(_reasonBox);
        Register(_typeBox);
        Register(_endBox);
        Register(_appealButton);
        Register(_exitButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.BannedAccountSubtitle;
    }

    protected override void ApplyTexts()
    {
        _notice.Text = TextCatalog.BannedAccountNotice;
        _reasonBox.Label = TextCatalog.BannedAccountReasonLabel;
        _typeBox.Label = TextCatalog.BannedAccountTypeLabel;
        _endBox.Label = TextCatalog.BannedAccountEndLabel;
        _appealButton.Title = TextCatalog.BannedAccountAppealButton;
        _exitButton.Title = TextCatalog.BannedAccountExitButton;
    }

    private void OnAppealClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Appeal);
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Navigator.Restart(ScreenId.Login);
    }

    private Rectangle GetRowBounds(int top, int rowTop)
    {
        return new Rectangle(ContentX, top + rowTop + LabelSpace, ContentWidth, Theme.FieldHeight);
    }
}
