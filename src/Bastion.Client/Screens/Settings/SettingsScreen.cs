using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Settings;

// The account and language panels of the settings. The account data (nickname,
// email, sessions, friend code) comes from the server, so it stays blank until then.
public sealed class SettingsScreen : FormScreen
{
    private const int CardHeight = Theme.WindowHeight - (Theme.PanelTop * 2);
    private const int SidebarWidth = 220;
    private const int SidebarGap = 60;
    private const int SidebarItemCount = 5;
    private const int SidebarItemSpacing = 4;
    private const int HeadingHeight = 34;
    private const int HeadingGap = 16;
    private const int TopBoxHeight = 96;
    private const int TopBoxAvatar = 56;
    private const int AvatarTextGap = 18;
    private const int NicknameTop = 24;
    private const int NicknameHeight = 24;
    private const int EmailTop = 52;
    private const int EmailHeight = 20;
    private const int EditButtonWidth = 100;
    private const int BlockGap = 16;
    private const int PasswordRowIndex = 0;
    private const int EmailRowIndex = 1;
    private const int SessionsRowIndex = 2;
    private const int FriendCodeRowIndex = 3;
    private const int SettingRowCount = 4;
    private const int RowHeight = 70;
    private const int SignOutHeight = 56;
    private const int SignOutGap = 24;
    private const int DeleteBoxHeight = 76;
    private const int LanguageRowHeight = 56;
    private const int LanguageLabelHeight = 22;
    private const int LanguageRowsGap = 10;
    private const int DividerWidth = 1;

    private const int BoardIndex = 0;
    private const int AudioIndex = 1;
    private const int AccountIndex = 2;
    private const int LanguageIndex = 3;
    private const int AccessibilityIndex = 4;

    private readonly SidebarMenu _sidebar;
    private readonly TextLine _heading;
    private readonly List<Control> _accountControls = [];
    private readonly List<Control> _languageControls = [];

    private readonly PanelBox _topBox;
    private readonly Avatar _avatar;
    private readonly TextLine _nickname;
    private readonly TextLine _email;
    private readonly Button _editButton;
    private readonly SettingRow _passwordRow;
    private readonly SettingRow _emailRow;
    private readonly SettingRow _sessionsRow;
    private readonly SettingRow _friendCodeRow;
    private readonly Button _signOutButton;
    private readonly ActionBox _deleteBox;

    private readonly TextLine _languageLabel;
    private readonly RadioRow _spanishRow;
    private readonly RadioRow _englishRow;
    private readonly TextLine _chatNote;

    public SettingsScreen(INavigator navigator, bool showsLanguage)
        : base(navigator, new CardShape
        {
            Width = Theme.PanelCardWidth,
            Height = CardHeight,
            Layout = ScreenLayout.PanelBare
        })
    {
        int top = PanelContentTop;
        int panelX = ContentX + SidebarWidth + SidebarGap;
        int panelWidth = ContentWidth - SidebarWidth - SidebarGap;

        _heading = new TextLine
        {
            Style = TextLineStyle.Heading,
            Bounds = new Rectangle(ContentX, top, SidebarWidth, HeadingHeight)
        };
        _sidebar = new SidebarMenu
        {
            SelectedIndex = showsLanguage ? LanguageIndex : AccountIndex,
            Bounds = new Rectangle(
                ContentX,
                top + HeadingHeight + HeadingGap,
                SidebarWidth,
                SidebarItemCount * (SidebarMenu.ItemHeight + SidebarItemSpacing))
        };
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.Items.Add(new SidebarItem());
        _sidebar.Items.Add(new SidebarItem());
        _sidebar.Items.Add(new SidebarItem { IsEnabled = false });
        _sidebar.ItemChosen += OnSidebarChosen;

        var topArea = new Rectangle(panelX, top, panelWidth, TopBoxHeight);
        _topBox = new PanelBox { Bounds = topArea };
        _avatar = new Avatar
        {
            Bounds = new Rectangle(
                topArea.X + PanelBox.Padding,
                topArea.Y + ((TopBoxHeight - TopBoxAvatar) / 2),
                TopBoxAvatar,
                TopBoxAvatar)
        };
        int textX = topArea.X + PanelBox.Padding + TopBoxAvatar + AvatarTextGap;
        _nickname = new TextLine { Bounds = new Rectangle(textX, topArea.Y + NicknameTop, panelWidth, NicknameHeight) };
        _email = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(textX, topArea.Y + EmailTop, panelWidth, EmailHeight)
        };
        _editButton = CreateOutlineButton(new Rectangle(
            topArea.Right - PanelBox.Padding - EditButtonWidth,
            topArea.Y + ((TopBoxHeight - Theme.SmallButtonHeight) / 2),
            EditButtonWidth,
            Theme.SmallButtonHeight));
        _editButton.Clicked += OnEditClicked;
        _avatar.Clicked += OnAvatarClicked;

        int rowsTop = topArea.Bottom + BlockGap;
        var rowArea = new Rectangle(panelX, rowsTop, panelWidth, RowHeight);
        _passwordRow = CreateRow(rowArea, PasswordRowIndex);
        _emailRow = CreateRow(rowArea, EmailRowIndex);
        _sessionsRow = CreateRow(rowArea, SessionsRowIndex);
        _friendCodeRow = CreateRow(rowArea, FriendCodeRowIndex);
        _friendCodeRow.IsButtonEnabled = false;
        _passwordRow.Clicked += OnPasswordRowClicked;
        _emailRow.Clicked += OnEmailRowClicked;
        _sessionsRow.Clicked += OnSessionsRowClicked;

        int signOutTop = rowsTop + (SettingRowCount * RowHeight) + SignOutGap;
        _signOutButton = CreateOutlineButton(new Rectangle(panelX, signOutTop, panelWidth, SignOutHeight));
        _signOutButton.Clicked += OnSignOutClicked;

        _deleteBox = new ActionBox
        {
            Bounds = new Rectangle(panelX, signOutTop + SignOutHeight + BlockGap, panelWidth, DeleteBoxHeight)
        };
        _deleteBox.Clicked += OnDeleteBoxClicked;

        _languageLabel = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(panelX, top, panelWidth, LanguageLabelHeight)
        };
        int rowTop = top + LanguageLabelHeight + LanguageRowsGap;
        _spanishRow = new RadioRow { Bounds = new Rectangle(panelX, rowTop, panelWidth, LanguageRowHeight) };
        _englishRow = new RadioRow
        {
            Bounds = new Rectangle(panelX, rowTop + LanguageRowHeight, panelWidth, LanguageRowHeight)
        };
        _spanishRow.Chosen += OnSpanishChosen;
        _englishRow.Chosen += OnEnglishChosen;
        _chatNote = new TextLine
        {
            Style = TextLineStyle.Small,
            Bounds = new Rectangle(
                panelX,
                Card.Bottom - Theme.CardPadding - LanguageLabelHeight,
                panelWidth,
                LanguageLabelHeight)
        };

        Register(_heading);
        Register(_sidebar);
        RegisterPanels();
        ShowPanel(showsLanguage);
        ApplyTexts();
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        base.Draw(canvas);
        int x = ContentX + SidebarWidth + (SidebarGap / 2);
        canvas.Shapes.DrawRectangle(
            new Rectangle(x, Card.Y + Theme.CardPadding, DividerWidth, Card.Height - (Theme.CardPadding * 2)),
            Theme.CheckBoxBorder);
    }

    protected override string GetSubtitle()
    {
        return string.Empty;
    }

    protected override void ApplyTexts()
    {
        _heading.Text = TextCatalog.SettingsHeading;
        _sidebar.Items[BoardIndex].Text = TextCatalog.SettingsBoardEntry;
        _sidebar.Items[AudioIndex].Text = TextCatalog.SettingsAudioEntry;
        _sidebar.Items[AccountIndex].Text = TextCatalog.SettingsAccountEntry;
        _sidebar.Items[LanguageIndex].Text = TextCatalog.SettingsLanguageEntry;
        _sidebar.Items[AccessibilityIndex].Text = TextCatalog.SettingsAccessibilityEntry;

        ApplyAccountTexts();
        ApplyLanguageTexts();
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

    private static SettingRow CreateRow(Rectangle area, int index)
    {
        return new SettingRow
        {
            Bounds = new Rectangle(area.X, area.Y + (index * RowHeight), area.Width, RowHeight)
        };
    }

    private void OnSidebarChosen(object? sender, SelectionChangedEventArgs e)
    {
        bool wantsLanguage = e.SelectedIndex == LanguageIndex;

        // Each panel is its own destination, so the back link and the
        // history treat them as the two screens the use cases describe.
        Navigator.GoTo(wantsLanguage ? ScreenId.SettingsLanguage : ScreenId.AccountSettings);
    }

    private void OnEditClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.EditProfile);
    }

    private void OnAvatarClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Profile);
    }

    private void OnPasswordRowClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangePassword);
    }

    private void OnEmailRowClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ChangeEmail);
    }

    private void OnSessionsRowClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ActiveSessions);
    }

    private void OnDeleteBoxClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.DeleteAccount);
    }

    private void OnSpanishChosen(object? sender, EventArgs e)
    {
        ChooseLanguage(LanguagePicker.SpanishIndex);
    }

    private void OnEnglishChosen(object? sender, EventArgs e)
    {
        ChooseLanguage(LanguagePicker.EnglishIndex);
    }

    private void OnSignOutClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.SettingsSignOutBody,
            PrimaryLabel = TextCatalog.SettingsSignOutButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = RestartAtLogin
        });
    }

    private void RestartAtLogin()
    {
        Navigator.Restart(ScreenId.Login);
    }

    private void ApplyAccountTexts()
    {
        _avatar.Text = TextCatalog.AvatarPlaceholder;
        _editButton.Title = TextCatalog.CommonEditButton;

        _passwordRow.Title = TextCatalog.SettingsPasswordRow;
        _passwordRow.ButtonLabel = TextCatalog.CommonChangeButton;
        _emailRow.Title = TextCatalog.SettingsEmailRow;
        _emailRow.ButtonLabel = TextCatalog.CommonChangeButton;
        _sessionsRow.Title = TextCatalog.SettingsSessionsRow;
        _sessionsRow.ButtonLabel = TextCatalog.CommonViewButton;
        _friendCodeRow.Title = TextCatalog.SettingsFriendCodeRow;
        _friendCodeRow.ButtonLabel = TextCatalog.CommonCopyButton;
        _signOutButton.Title = TextCatalog.SettingsSignOutButton;
        _deleteBox.Title = TextCatalog.SettingsDeleteTitle;
        _deleteBox.Hint = TextCatalog.SettingsDeleteHint;
    }

    private void ApplyLanguageTexts()
    {
        bool isEnglish = Language.IsEnglish;

        _languageLabel.Text = TextCatalog.SettingsInterfaceLanguageLabel;
        _spanishRow.Text = TextCatalog.SpanishMexicoLanguageName;
        _englishRow.Text = TextCatalog.EnglishLanguageName;
        _spanishRow.IsSelected = !isEnglish;
        _englishRow.IsSelected = isEnglish;
        _spanishRow.Tag = isEnglish ? string.Empty : TextCatalog.SettingsCurrentTag;
        _englishRow.Tag = isEnglish ? TextCatalog.SettingsCurrentTag : string.Empty;
        _chatNote.Text = TextCatalog.SettingsChatNote;
    }

    private void ChooseLanguage(int index)
    {
        LanguagePicker.Apply(index);
        ApplyTexts();
    }

    private void RegisterPanels()
    {
        Control[] accountControls =
        [
            _topBox,
            _avatar,
            _nickname,
            _email,
            _editButton,
            _passwordRow,
            _emailRow,
            _sessionsRow,
            _friendCodeRow,
            _signOutButton,
            _deleteBox
        ];
        Control[] languageControls = [_languageLabel, _spanishRow, _englishRow, _chatNote];

        _accountControls.AddRange(accountControls);
        _languageControls.AddRange(languageControls);
        _accountControls.ForEach(Register);
        _languageControls.ForEach(Register);
    }

    private void ShowPanel(bool showsLanguage)
    {
        foreach (Control control in _accountControls)
        {
            SetVisible(control, !showsLanguage);
        }

        foreach (Control control in _languageControls)
        {
            SetVisible(control, showsLanguage);
        }
    }
}
