using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;
using Bastion.Client.Validation;

namespace Bastion.Client.Screens.EditProfile;

public sealed class EditProfileScreen : FormScreen
{
    private const int AvatarSize = 64;
    private const int IconRows = 2;
    private const int IconRowGap = 8;
    private const int AvatarGap = 16;
    private const int AvatarToIconsGap = 12;
    private const int IconsToHintGap = 8;
    private const int SmallButtonWidth = 170;
    private const int SmallButtonGap = 10;
    private const int HintHeight = 20;
    private const int FieldToHintGap = 6;
    private const int SectionGap = 18;
    private const int ChangeButtonWidth = 120;
    private const int PreviewHeight = 64;
    private const int PreviewAvatar = 40;
    private const int PreviewInset = 14;
    private const int PreviewLineTop = 12;
    private const int PreviewLineHeight = 22;
    private const int PreviewHintTop = 36;
    private const int ToggleHeight = 40;
    private const int MaxLinks = 4;
    private const int LinkRowHeight = 46;
    private const int LinkRowGap = 8;
    private const int LinkTypeWidth = 130;
    private const int LinkRemoveWidth = 46;
    private const int MaxLinkLength = 254;
    private const int ButtonsGap = 24;
    private const int NoSelection = -1;

    private const int IconsRowTop = AvatarSize + AvatarToIconsGap;
    private const int IconsRowsHeight = (IconRows * Theme.ChipHeight) + ((IconRows - 1) * IconRowGap);
    private const int IconsHintTop = IconsRowTop + IconsRowsHeight + IconsToHintGap;
    private const int AvatarBlockHeight = IconsHintTop + HintHeight;
    private const int NameLabelTop = AvatarBlockHeight + SectionGap;
    private const int NameFieldTop = NameLabelTop + LabelSpace;
    private const int NameHintTop = NameFieldTop + Theme.FieldHeight + FieldToHintGap;
    private const int TitleLabelTop = NameHintTop + HintHeight + SectionGap;
    private const int TitleChipsTop = TitleLabelTop + LabelSpace;
    private const int PreviewTop = TitleChipsTop + Theme.ChipHeight + SectionGap;
    private const int LeftHeight = PreviewTop + PreviewHeight;

    private const int LanguageLabelTop = ToggleHeight + SectionGap;
    private const int LanguageFieldTop = LanguageLabelTop + LabelSpace;
    private const int LinksLabelTop = LanguageFieldTop + Theme.FieldHeight + SectionGap;
    private const int LinksRowsTop = LinksLabelTop + LabelSpace;
    private const int LinksHintTop = LinksRowsTop + (MaxLinks * (LinkRowHeight + LinkRowGap));
    private const int RightHeight = LinksHintTop + HintHeight;

    private const int ContentHeight = RightHeight > LeftHeight ? RightHeight : LeftHeight;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + ContentHeight + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly Avatar _avatar;
    private readonly Button _uploadButton;
    private readonly Button _iconsButton;
    private readonly ChipRow _iconChips;
    private readonly TextLine _iconsHint;
    private readonly ValueBox _nicknameBox;
    private readonly Button _changeNicknameButton;
    private readonly TextLine _nicknameHint;
    private readonly TextLine _titleLabel;
    private readonly ChipRow _titleChips;
    private readonly PanelBox _previewBox;
    private readonly Avatar _previewAvatar;
    private readonly TextLine _previewLine;
    private readonly TextLine _previewHint;
    private readonly ToggleSwitch _spectatorsSwitch;
    private readonly Selector _languageSelector;
    private readonly TextLine _linksLabel;
    private readonly List<DropDown> _linkTypes = [];
    private readonly List<TextField> _linkFields = [];
    private readonly List<Button> _linkRemoveButtons = [];
    private readonly TextLine _linksHint;
    private readonly Button _saveButton;
    private readonly Button _cancelButton;
    private int _titleIndex = NoSelection;
    private int _iconIndex = NoSelection;
    private bool _hasValidated;

    public EditProfileScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = Theme.PanelCardWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        int top = PanelContentTop;
        int leftX = ContentX;
        int rightX = ContentX + ColumnWidth + Gutter;
        int besideAvatar = leftX + AvatarSize + AvatarGap;

        _avatar = new Avatar { Bounds = new Rectangle(leftX, top, AvatarSize, AvatarSize) };
        int buttonsTop = top + ((AvatarSize - Theme.SmallButtonHeight) / 2);
        _uploadButton = CreateOutlineButton(
            new Rectangle(besideAvatar, buttonsTop, SmallButtonWidth, Theme.SmallButtonHeight));
        _uploadButton.IsEnabled = false;
        _iconsButton = CreateOutlineButton(
            new Rectangle(
                besideAvatar + SmallButtonWidth + SmallButtonGap,
                buttonsTop,
                SmallButtonWidth,
                Theme.SmallButtonHeight));
        _iconChips = new ChipRow { Bounds = new Rectangle(leftX, top + IconsRowTop, ColumnWidth, IconsRowsHeight) };
        _iconChips.ChipChosen += OnIconChosen;
        _iconsHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(leftX, top + IconsHintTop, ColumnWidth, HintHeight)
        };

        _nicknameBox = new ValueBox
        {
            Bounds = new Rectangle(
                leftX,
                top + NameFieldTop,
                ColumnWidth - ChangeButtonWidth - SmallButtonGap,
                Theme.FieldHeight)
        };
        _changeNicknameButton = CreateOutlineButton(
            new Rectangle(
                leftX + ColumnWidth - ChangeButtonWidth,
                top + NameFieldTop,
                ChangeButtonWidth,
                Theme.FieldHeight));
        _changeNicknameButton.Clicked += OnChangeNicknameClicked;
        _nicknameHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(leftX, top + NameHintTop, ColumnWidth, HintHeight)
        };

        _titleLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(leftX, top + TitleLabelTop, ColumnWidth, LabelSpace)
        };
        _titleChips = new ChipRow { Bounds = new Rectangle(leftX, top + TitleChipsTop, ColumnWidth, Theme.ChipHeight) };
        _titleChips.ChipChosen += OnTitleChosen;

        var previewArea = new Rectangle(leftX, top + PreviewTop, ColumnWidth, PreviewHeight);
        int previewTextX = previewArea.X + PreviewInset + PreviewAvatar + PreviewInset;
        _previewBox = new PanelBox { Bounds = previewArea };
        _previewAvatar = new Avatar
        {
            Bounds = new Rectangle(
                previewArea.X + PreviewInset,
                previewArea.Y + ((PreviewHeight - PreviewAvatar) / 2),
                PreviewAvatar,
                PreviewAvatar)
        };
        _previewLine = new TextLine
        {
            Bounds = new Rectangle(previewTextX, previewArea.Y + PreviewLineTop, ColumnWidth, PreviewLineHeight)
        };
        _previewHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(previewTextX, previewArea.Y + PreviewHintTop, ColumnWidth, HintHeight)
        };

        _spectatorsSwitch = new ToggleSwitch { Bounds = new Rectangle(rightX, top, ColumnWidth, ToggleHeight) };
        _languageSelector = new Selector
        {
            Options = LanguagePicker.GetNames(),
            SelectedIndex = Language.IsEnglish ? LanguagePicker.EnglishIndex : LanguagePicker.SpanishIndex,
            Bounds = new Rectangle(rightX, top + LanguageFieldTop, ColumnWidth, Theme.FieldHeight)
        };
        _linksLabel = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(rightX, top + LinksLabelTop, ColumnWidth, LabelSpace)
        };

        for (int linkIndex = 0; linkIndex < MaxLinks; linkIndex++)
        {
            CreateLinkRow(rightX, top + LinksRowsTop + (linkIndex * (LinkRowHeight + LinkRowGap)));
        }

        _linksHint = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(rightX, top + LinksHintTop, ColumnWidth, HintHeight)
        };

        _saveButton = CreatePrimaryButton(false);
        _cancelButton = CreateOutlineButton(SecondaryButtonBounds);
        _saveButton.Clicked += OnSaveClicked;
        _cancelButton.Clicked += OnCancelClicked;

        RegisterControls();
        ShowNextEmptyLinkRow();
        ApplyTexts();
    }

    public override void Update(InputState input)
    {
        base.Update(input);
        ShowNextEmptyLinkRow();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.EditProfileSubtitle;
    }

    protected override void ApplyTexts()
    {
        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _previewAvatar.Text = TextCatalog.AvatarPlaceholder;
        _uploadButton.Title = TextCatalog.EditProfileUploadButton;
        _iconsButton.Title = TextCatalog.EditProfileIconsButton;
        _iconsHint.Text = TextCatalog.EditProfileIconsHint;
        RefreshIconChips();

        _nicknameBox.Label = TextCatalog.EditProfileNicknameLabel;
        _changeNicknameButton.Title = TextCatalog.CommonChangeButton;
        _nicknameHint.Text = TextCatalog.EditProfileNicknameHint;

        _titleLabel.Text = TextCatalog.EditProfileTitleLabel;
        RefreshTitleChips();
        RefreshPreview();
        _previewHint.Text = TextCatalog.EditProfilePreviewHint;

        _spectatorsSwitch.Text = TextCatalog.EditProfileSpectatorsText;
        _languageSelector.Label = TextCatalog.EditProfileLanguageLabel;
        _languageSelector.Options = LanguagePicker.GetNames();
        ApplyLinkTexts();

        _saveButton.Title = TextCatalog.CommonSaveButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;

        if (_hasValidated)
        {
            Validate();
        }
    }

    private static IReadOnlyList<string> GetTitleNames()
    {
        return [TextCatalog.TitleRookie, TextCatalog.TitleStrategist, TextCatalog.TitleBuilder];
    }

    private static IReadOnlyList<string> GetLinkTypeNames()
    {
        return
        [
            TextCatalog.LinkTypeTwitch,
            TextCatalog.LinkTypeYoutube,
            TextCatalog.LinkTypeDiscord,
            TextCatalog.LinkTypeOther
        ];
    }

    private static IReadOnlyList<string> GetIconNames()
    {
        return
        [
            TextCatalog.IconFox,
            TextCatalog.IconOwl,
            TextCatalog.IconBear,
            TextCatalog.IconCat,
            TextCatalog.IconWall,
            TextCatalog.IconPawn,
            TextCatalog.IconLighthouse
        ];
    }

    private static string? GetLinkWarning(string link)
    {
        if (link.Length == 0)
        {
            return null;
        }

        if (!InputRules.IsWebAddress(link))
        {
            return TextCatalog.EditProfileLinkInvalid;
        }

        return InputRules.IsShortener(link) ? TextCatalog.EditProfileLinkShortener : null;
    }

    private static bool IsFieldHidden(TextField field)
    {
        return !field.IsVisible;
    }

    private static bool IsFieldFilled(TextField field)
    {
        return field.IsVisible && field.Text.Trim().Length > 0;
    }

    private static void SetVisible(Control control, bool isVisible)
    {
        if (isVisible)
        {
            control.Show();
        }
        else
        {
            control.Hide();
        }
    }

    private void OnIconChosen(object? sender, SelectionChangedEventArgs e)
    {
        if (e.SelectedIndex < _iconChips.Items.Count - 1)
        {
            _iconIndex = e.SelectedIndex;
            RefreshIconChips();
        }
    }

    private void OnTitleChosen(object? sender, SelectionChangedEventArgs e)
    {
        _titleIndex = e.SelectedIndex < GetTitleNames().Count ? e.SelectedIndex : NoSelection;
        RefreshTitleChips();
        RefreshPreview();
    }

    private void OnChangeNicknameClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangeNickname);
    }

    private void OnRemoveLinkClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        int linkIndex = _linkRemoveButtons.IndexOf(button);

        if (linkIndex >= 0)
        {
            RemoveLink(linkIndex);
        }
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        _hasValidated = true;

        if (!Validate())
        {
            return;
        }

        bool wantsEnglish = _languageSelector.SelectedIndex == LanguagePicker.EnglishIndex;

        if (wantsEnglish != Language.IsEnglish)
        {
            LanguagePicker.Apply(_languageSelector.SelectedIndex);
        }

        Navigator.ReturnTo(ScreenId.Profile);
        Navigator.ShowMessage(DialogTone.Success, TextCatalog.EditProfileSavedBody);
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void CreateLinkRow(int rightX, int rowTop)
    {
        int fieldX = rightX + LinkTypeWidth + SmallButtonGap;
        int fieldWidth = ColumnWidth - LinkTypeWidth - LinkRemoveWidth - (SmallButtonGap * 2);
        IReadOnlyList<string> linkTypes = GetLinkTypeNames();

        var type = new DropDown
        {
            Options = linkTypes,
            SelectedIndex = linkTypes.Count - 1,
            IsVisible = false,
            Bounds = new Rectangle(rightX, rowTop, LinkTypeWidth, LinkRowHeight)
        };
        var field = new TextField
        {
            MaxLength = MaxLinkLength,
            IsVisible = false,
            Bounds = new Rectangle(fieldX, rowTop, fieldWidth, LinkRowHeight)
        };
        Button remove = CreateOutlineButton(
            new Rectangle(fieldX + fieldWidth + SmallButtonGap, rowTop, LinkRemoveWidth, LinkRowHeight));
        remove.Hide();
        remove.Clicked += OnRemoveLinkClicked;

        _linkTypes.Add(type);
        _linkFields.Add(field);
        _linkRemoveButtons.Add(remove);
    }

    private void RegisterControls()
    {
        Register(_avatar);
        Register(_uploadButton);
        Register(_iconsButton);
        Register(_iconChips);
        Register(_iconsHint);
        Register(_nicknameBox);
        Register(_changeNicknameButton);
        Register(_nicknameHint);
        Register(_titleLabel);
        Register(_titleChips);
        Register(_previewBox);
        Register(_previewAvatar);
        Register(_previewLine);
        Register(_previewHint);
        Register(_spectatorsSwitch);
        Register(_languageSelector);
        Register(_linksLabel);

        for (int linkIndex = 0; linkIndex < MaxLinks; linkIndex++)
        {
            RegisterField(_linkFields[linkIndex]);
            Register(_linkRemoveButtons[linkIndex]);
        }

        Register(_linksHint);
        Register(_saveButton);
        Register(_cancelButton);
        _linkTypes.ForEach(Register);
    }

    private void ApplyLinkTexts()
    {
        _linksLabel.Text = TextCatalog.EditProfileLinksLabel;
        IReadOnlyList<string> linkTypes = GetLinkTypeNames();

        for (int linkIndex = 0; linkIndex < MaxLinks; linkIndex++)
        {
            _linkTypes[linkIndex].Options = linkTypes;
            _linkFields[linkIndex].Placeholder = TextCatalog.EditProfileLinkPlaceholder;
            _linkRemoveButtons[linkIndex].Title = TextCatalog.EditProfileRemoveLinkButton;
        }

        _linksHint.Text = TextCatalog.EditProfileLinksHint;
    }

    private bool Validate()
    {
        bool isValid = true;

        foreach (TextField field in _linkFields)
        {
            field.Warning = field.IsVisible ? GetLinkWarning(field.Text.Trim()) : null;
            isValid &= !field.HasWarning;
        }

        return isValid;
    }

    private void ShowNextEmptyLinkRow()
    {
        int hiddenIndex = _linkFields.FindIndex(IsFieldHidden);
        bool isPreviousFilled = hiddenIndex == 0 || (hiddenIndex > 0 && IsFieldFilled(_linkFields[hiddenIndex - 1]));

        if (isPreviousFilled)
        {
            SetLinkRowVisible(hiddenIndex, true);
        }

        for (int linkIndex = 0; linkIndex < MaxLinks; linkIndex++)
        {
            SetVisible(_linkRemoveButtons[linkIndex], IsFieldFilled(_linkFields[linkIndex]));
        }
    }

    private void RemoveLink(int index)
    {
        for (int linkIndex = index; linkIndex < MaxLinks - 1; linkIndex++)
        {
            _linkFields[linkIndex].SetText(_linkFields[linkIndex + 1].Text);
            _linkTypes[linkIndex].SelectedIndex = _linkTypes[linkIndex + 1].SelectedIndex;
            _linkFields[linkIndex].Warning = _linkFields[linkIndex + 1].Warning;
        }

        int lastIndex = MaxLinks - 1;
        _linkFields[lastIndex].SetText(string.Empty);
        _linkFields[lastIndex].Warning = null;

        for (int linkIndex = MaxLinks - 1; linkIndex > 0; linkIndex--)
        {
            bool isEmptyAfterEmpty = _linkFields[linkIndex].Text.Length == 0
                && _linkFields[linkIndex - 1].Text.Length == 0;

            if (_linkFields[linkIndex].IsVisible && isEmptyAfterEmpty)
            {
                SetLinkRowVisible(linkIndex, false);
            }
        }
    }

    private void SetLinkRowVisible(int index, bool isVisible)
    {
        SetVisible(_linkTypes[index], isVisible);
        SetVisible(_linkFields[index], isVisible);
        SetVisible(_linkRemoveButtons[index], isVisible);
    }

    private void RefreshIconChips()
    {
        _iconChips.Items.Clear();
        IReadOnlyList<string> icons = GetIconNames();

        for (int iconIndex = 0; iconIndex < icons.Count; iconIndex++)
        {
            _iconChips.Items.Add(new Chip { Text = icons[iconIndex], IsSelected = iconIndex == _iconIndex });
        }

        _iconChips.Items.Add(new Chip { Text = TextCatalog.EditProfileMoreIconsChip, IsDashed = true });
    }

    private void RefreshTitleChips()
    {
        _titleChips.Items.Clear();
        IReadOnlyList<string> titles = GetTitleNames();

        for (int titleIndex = 0; titleIndex < titles.Count; titleIndex++)
        {
            _titleChips.Items.Add(new Chip { Text = titles[titleIndex], IsSelected = titleIndex == _titleIndex });
        }

        _titleChips.Items.Add(new Chip
        {
            Text = TextCatalog.ProfileNoTitle,
            IsDashed = _titleIndex >= 0,
            IsSelected = _titleIndex < 0
        });
    }

    private void RefreshPreview()
    {
        IReadOnlyList<string> titles = GetTitleNames();
        _previewLine.Text = _titleIndex >= 0 && _titleIndex < titles.Count
            ? titles[_titleIndex]
            : TextCatalog.ProfileNoTitle;
    }
}
