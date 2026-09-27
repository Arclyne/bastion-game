using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Validation;

namespace Bastion.Client.Screens.ChangeNickname;

public sealed class ChangeNicknameScreen : FormScreen
{
    private const int MaxNicknameLength = 30;
    private const int FieldGap = 26;
    private const int NoticeHeight = 76;
    private const int ButtonsGap = 24;
    private const int ContentHeight =
        LabelSpace + Theme.FieldHeight + FieldGap
        + LabelSpace + Theme.FieldHeight + FieldGap + NoticeHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly ValueBox _currentBox;
    private readonly TextField _newField;
    private readonly NoticeBox _notice;
    private readonly Button _continueButton;
    private readonly Button _cancelButton;
    private bool _hasValidated;

    public ChangeNicknameScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = PanelNarrowWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        int top = PanelContentTop + LabelSpace;
        _currentBox = new ValueBox { Bounds = new Rectangle(ContentX, top, ContentWidth, Theme.FieldHeight) };

        int newTop = top + Theme.FieldHeight + FieldGap + LabelSpace;
        _newField = new TextField
        {
            MaxLength = MaxNicknameLength,
            Bounds = new Rectangle(ContentX, newTop, ContentWidth, Theme.FieldHeight)
        };

        int noticeTop = newTop + Theme.FieldHeight + FieldGap;
        _notice = new NoticeBox
        {
            IsCritical = true,
            Bounds = new Rectangle(ContentX, noticeTop, ContentWidth, NoticeHeight)
        };

        _continueButton = CreatePrimaryButton(true);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _continueButton.Clicked += OnContinueClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_currentBox);
        RegisterField(_newField);
        Register(_notice);
        Register(_continueButton);
        Register(_cancelButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ChangeNicknameSubtitle;
    }

    protected override void ApplyTexts()
    {
        _currentBox.Label = TextCatalog.ChangeNicknameCurrentLabel;
        _newField.Label = TextCatalog.ChangeNicknameNewLabel;
        _newField.Placeholder = TextCatalog.ChangeNicknameNewPlaceholder;
        _notice.Text = TextCatalog.ChangeNicknameNotice;
        _continueButton.Title = TextCatalog.CommonContinueButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private void OnContinueClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        string newNickname = _newField.Text.Trim();
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Title = TextCatalog.ChangeNicknameConfirmTitle,
            Body = string.Format(
                CultureInfo.CurrentCulture,
                TextCatalog.ChangeNicknameConfirmFormat,
                _currentBox.Value,
                newNickname),
            Detail = TextCatalog.ChangeNicknameConfirmDetail,
            PrimaryLabel = TextCatalog.ChangeNicknameConfirmButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = Apply
        });
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void Apply()
    {
        Navigator.ReturnTo(ScreenId.Profile);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.ChangeNicknameDoneBody);
    }

    // Whether the nickname is already taken is answered by the server.
    private bool Validate()
    {
        _newField.Warning = InputRules.HasNicknameLength(_newField.Text.Trim())
            ? null
            : TextCatalog.RegisterNicknameLength;

        return !_newField.HasWarning;
    }
}
