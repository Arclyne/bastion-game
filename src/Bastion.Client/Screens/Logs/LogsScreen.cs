using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Logs;

// CU-48 main flow steps 1 and 4. Which log, the date range and the filters on
// top; the records and their counts below.
public sealed class LogsScreen : FormScreen
{
    private const int SectionGap = 20;
    private const int RowHeight = 50;
    private const int RowGap = 8;
    private const int VisibleRows = 3;
    private const int FilterColumns = 3;
    private const int FilterGaps = FilterColumns - 1;
    private const int LabelRowsAboveList = 2;

    // The filter row and its label sit above the list, inside the same card.
    private static readonly int _cardHeight =
        ComputeListCardHeight(VisibleRows, RowHeight, RowGap)
        + (LabelSpace * LabelRowsAboveList) + Theme.FieldHeight + SectionGap;

    private readonly Selector _logSelector;
    private readonly TextField _fromField;
    private readonly TextField _toField;
    private readonly List<DataRow> _rows = [];
    private readonly TextBlock _emptyNotice;
    private readonly Button _searchButton;
    private readonly Button _backButton;

    public LogsScreen(INavigator navigator)
        : base(navigator, WideCardWidth, _cardHeight)
    {
        int column = (ContentWidth - (SectionGap * FilterGaps)) / FilterColumns;

        _logSelector = new Selector
        {
            Options = BuildLogs(),
            Bounds = new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight)
        };

        _fromField = CreateDateField(ContentX + column + SectionGap, column);
        _toField = CreateDateField(ContentX + ((column + SectionGap) * FilterGaps), column);

        int rowsTop = FirstRowTop + Theme.FieldHeight + SectionGap + LabelSpace;

        // The rows stay hidden until the server sends the records.
        for (int rowIndex = 0; rowIndex < VisibleRows; rowIndex++)
        {
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = new Rectangle(ContentX, rowsTop + (rowIndex * (RowHeight + RowGap)), ContentWidth, RowHeight)
            };

            _rows.Add(row);
            Register(row);
        }

        _emptyNotice = new TextBlock
        {
            IsSmall = true,
            Bounds = new Rectangle(ContentX, rowsTop, ContentWidth, RowHeight)
        };

        _searchButton = CreatePrimaryButton(true);
        _backButton = CreateSecondaryButton();
        LayOutActionsInRow([_searchButton, _backButton]);
        _searchButton.Clicked += OnSearchClicked;
        _backButton.Clicked += OnBackClicked;

        Register(_logSelector);
        RegisterField(_fromField);
        RegisterField(_toField);
        Register(_emptyNotice);
        Register(_searchButton);
        Register(_backButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.LogsSubtitle;
    }

    protected override void ApplyTexts()
    {
        _logSelector.Label = TextCatalog.LogsWhichLabel;
        _logSelector.Options = BuildLogs();
        _fromField.Label = TextCatalog.LogsFromLabel;
        _fromField.Placeholder = TextCatalog.LogsDatePlaceholder;
        _toField.Label = TextCatalog.LogsToLabel;
        _toField.Placeholder = TextCatalog.LogsDatePlaceholder;
        _emptyNotice.Text = TextCatalog.LogsEmpty;
        _searchButton.Title = TextCatalog.LogsSearchButton;
        _backButton.Title = TextCatalog.CommonBackButton;
    }

    private static IReadOnlyList<string> BuildLogs()
    {
        return [TextCatalog.LogsAccessOption, TextCatalog.LogsModerationOption];
    }

    // The search runs on the server, which does not expose the logs yet.
    private void OnSearchClicked(object? sender, EventArgs e)
    {
    }

    private void OnBackClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private TextField CreateDateField(int x, int width)
    {
        return new TextField
        {
            IsCentered = true,
            Bounds = new Rectangle(x, FirstRowTop, width, Theme.FieldHeight)
        };
    }
}
