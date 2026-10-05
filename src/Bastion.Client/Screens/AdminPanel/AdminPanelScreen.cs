using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.AdminPanel;

public sealed class AdminPanelScreen : FormScreen
{
    private const int CardHeight = 392;
    private const int SectionGap = 20;
    private const int RowHeight = 54;
    private const int RowGap = 8;
    private const int VisibleRows = 2;
    private const int ColumnCount = 2;
    private const int HistoryRow = 2;

    private readonly ValueBox _nicknameBox;
    private readonly ValueBox _registeredBox;
    private readonly ValueBox _stateBox;
    private readonly ValueBox _typeBox;
    private readonly List<DataRow> _history = [];
    private readonly Button _grantButton;
    private readonly Button _queueButton;
    private readonly Button _logsButton;
    private readonly Button _backButton;

    public AdminPanelScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / ColumnCount;

        _nicknameBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight));
        _registeredBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop, column, Theme.FieldHeight));
        _stateBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop + RowSpacing, column, Theme.FieldHeight));
        _typeBox = CreateValueBox(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop + RowSpacing, column, Theme.FieldHeight));

        int historyTop = FirstRowTop + (RowSpacing * HistoryRow);

        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = new Rectangle(
                    ContentX,
                    historyTop + (rowIndex * (RowHeight + RowGap)),
                    ContentWidth,
                    RowHeight)
            };

            _history.Add(row);
            Register(row);
        }

        _grantButton = CreatePrimaryButton(false);
        _queueButton = CreateSecondaryButton();
        _logsButton = CreateSecondaryButton();
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_grantButton, _queueButton, _logsButton, _backButton]);
        _grantButton.Clicked += OnGrantClicked;
        _queueButton.Clicked += OnQueueClicked;
        _logsButton.Clicked += OnLogsClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_nicknameBox);
        Register(_registeredBox);
        Register(_stateBox);
        Register(_typeBox);
        Register(_grantButton);
        Register(_queueButton);
        Register(_logsButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AdminPanelSubtitle;
    }

    protected override void ApplyTexts()
    {
        _nicknameBox.Label = TextCatalog.AdminPanelNicknameLabel;
        _registeredBox.Label = TextCatalog.AdminPanelRegisteredLabel;
        _stateBox.Label = TextCatalog.AdminPanelStateLabel;
        _typeBox.Label = TextCatalog.AdminPanelTypeLabel;
        _grantButton.Title = TextCatalog.AdminPanelGrantButton;
        _queueButton.Title = TextCatalog.AdminPanelQueueButton;
        _logsButton.Title = TextCatalog.AdminPanelLogsButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnGrantClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.AdminPanelGrantConfirmBody,
            PrimaryLabel = TextCatalog.AdminPanelGrantButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = GoBack
        });
    }

    private void OnQueueClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.ModerationQueue);
    }

    private void OnLogsClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.Logs);
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void GoBack()
    {
        Navigator.GoBack();
    }
}
