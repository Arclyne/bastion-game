using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.AddFriend;

public sealed class AddFriendScreen : FormScreen
{
    private const int CardHeight = 380;
    private const int MaxSearchLength = 30;
    private const int RowHeight = 58;
    private const int RowGap = 10;
    private const int VisibleRows = 3;
    private const int ListGap = 26;

    private readonly TextField _searchField;
    private readonly List<DataRow> _rows = [];
    private readonly Button _searchButton;
    private readonly Button _backButton;

    public AddFriendScreen(INavigator navigator)
        : base(navigator, NarrowCardWidth, CardHeight)
    {
        _searchField = new TextField { MaxLength = MaxSearchLength, Bounds = GetRow(0) };

        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow { IsVisible = false, Bounds = GetSuggestionBounds(rowIndex) };
            _rows.Add(row);
            Register(row);
        }

        _searchButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        _searchButton.Clicked += OnSearchClicked;
        _backButton.Clicked += OnBackClicked;

        RegisterField(_searchField);
        Register(_searchButton);
        Register(_backButton);

        ApplyTexts();
        FocusFirstField();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.AddFriendSubtitle;
    }

    protected override void ApplyTexts()
    {
        _searchField.Label = TextCatalog.AddFriendSearchLabel;
        _searchField.Placeholder = TextCatalog.AddFriendSearchPlaceholder;

        foreach (DataRow row in _rows)
        {
            row.Subtitle = TextCatalog.AddFriendSuggestionReason;
        }

        _searchButton.Title = TextCatalog.AddFriendSearchButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private Rectangle GetSuggestionBounds(int index)
    {
        int top = GetRow(0).Bottom + ListGap + (index * (RowHeight + RowGap));

        return new Rectangle(ContentX, top, ContentWidth, RowHeight);
    }
}
