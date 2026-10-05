using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.ActiveSessions;

public sealed class ActiveSessionsScreen : FormScreen
{
    private const int RowHeight = 64;
    private const int RowGap = 12;
    private const int VisibleRows = 3;
    private const int CurrentRowIndex = 0;
    private const int ButtonsGap = 24;
    private const int CardHeight =
        Theme.CardPadding + Theme.PanelBackHeight + Theme.PanelGap + Theme.PanelTitleHeight + Theme.PanelGap
        + (VisibleRows * RowHeight) + ((VisibleRows - 1) * RowGap)
        + ButtonsGap + Theme.PanelButtonHeight + Theme.CardPadding;

    private readonly List<SessionRow> _rows = [];
    private readonly TextLine _emptyLine;
    private readonly Button _closeAllButton;

    public ActiveSessionsScreen(INavigator navigator)
        : base(navigator, new CardShape
        {
            Width = PanelNarrowWidth,
            Height = CardHeight,
            Layout = ScreenLayout.Panel
        })
    {
        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            int rowTop = PanelContentTop + (rowIndex * (RowHeight + RowGap));
            var row = new SessionRow
            {
                IsCurrent = rowIndex == CurrentRowIndex,
                IsVisible = false,
                Bounds = new Rectangle(ContentX, rowTop, ContentWidth, RowHeight)
            };
            row.CloseRequested += OnCloseRequested;
            _rows.Add(row);
            Register(row);
        }

        _emptyLine = new TextLine
        {
            Style = TextLineStyle.Muted,
            IsCentered = true,
            Bounds = new Rectangle(ContentX, PanelContentTop, ContentWidth, RowHeight)
        };
        Register(_emptyLine);

        _closeAllButton = CreatePrimaryButton(false);
        _closeAllButton.Clicked += OnCloseAllClicked;
        Register(_closeAllButton);

        ApplyTexts();
    }

    public override void Update(InputState input)
    {
        base.Update(input);

        if (_rows.Exists(IsRowVisible))
        {
            _emptyLine.Hide();
            return;
        }

        _emptyLine.Show();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ActiveSessionsSubtitle;
    }

    protected override void ApplyTexts()
    {
        foreach (SessionRow row in _rows)
        {
            row.CloseLabel = TextCatalog.ActiveSessionsCloseButton;
            row.CurrentLabel = TextCatalog.ActiveSessionsCurrentBadge;
        }

        _emptyLine.Text = TextCatalog.ActiveSessionsEmpty;
        _closeAllButton.Title = TextCatalog.ActiveSessionsCloseAllButton;
    }

    private static bool IsRowVisible(SessionRow row)
    {
        return row.IsVisible;
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        if (sender is not SessionRow row)
        {
            return;
        }

        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = string.Format(
                CultureInfo.CurrentCulture,
                TextCatalog.ActiveSessionsCloseBody,
                row.Device,
                row.LastUse),
            PrimaryLabel = TextCatalog.SettingsSignOutButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = row.Hide
        });
    }

    private void OnCloseAllClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.ActiveSessionsCloseAllBody,
            PrimaryLabel = TextCatalog.ActiveSessionsCloseAllButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = CloseOthers
        });
    }

    private void CloseOthers()
    {
        foreach (SessionRow row in _rows)
        {
            if (!row.IsCurrent)
            {
                row.Hide();
            }
        }

        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.ActiveSessionsChangePasswordBody,
            PrimaryLabel = TextCatalog.CommonChangeButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = OpenChangePassword
        });
    }

    private void OpenChangePassword()
    {
        Navigator.GoTo(ScreenId.ChangePassword);
    }
}
