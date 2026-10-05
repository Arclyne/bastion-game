using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Friends;

public sealed class FriendsScreen : FormScreen
{
    private const int CardHeight = 380;
    private const int RowHeight = 62;
    private const int RowGap = 10;
    private const int VisibleRows = 5;

    private readonly List<DataRow> _rows = [];
    private readonly TextLine _emptyState;
    private readonly Button _addButton;
    private readonly Button _backButton;

    public FriendsScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow { IsVisible = false, Bounds = GetRowBounds(rowIndex) };
            row.Clicked += OnRowClicked;
            _rows.Add(row);
            Register(row);
        }

        _emptyState = new TextLine { Style = TextLineStyle.Muted, Bounds = GetRowBounds(0) };
        _addButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _addButton.Clicked += OnAddClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_emptyState);
        Register(_addButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.FriendsSubtitle;
    }

    protected override void ApplyTexts()
    {
        _emptyState.Text = TextCatalog.FriendsEmpty;
        _addButton.Title = TextCatalog.FriendsAddButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnRowClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.PlayerCard);
    }

    private void OnAddClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AddFriend);
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
