using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.ApplySanction;

public sealed class ApplySanctionScreen : FormScreen
{
    private const int CardHeight = 400;
    private const int SectionGap = 20;
    private const int HistoryHeight = 54;
    private const int HistoryGap = 8;
    private const int VisibleHistory = 2;
    private const int ColumnCount = 2;
    private const int HistoryRow = 2;

    private readonly ValueBox _playerBox;
    private readonly Selector _scopeSelector;
    private readonly Selector _typeSelector;
    private readonly Selector _durationSelector;
    private readonly List<DataRow> _history = [];
    private readonly Button _applyButton;
    private readonly Button _cancelButton;

    public ApplySanctionScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int column = (ContentWidth - SectionGap) / ColumnCount;

        _playerBox = CreateValueBox(new Rectangle(ContentX, FirstRowTop, column, Theme.FieldHeight));
        _scopeSelector = CreateSelector(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop, column, Theme.FieldHeight),
            BuildScopes());
        _typeSelector = CreateSelector(
            new Rectangle(ContentX, FirstRowTop + RowSpacing, column, Theme.FieldHeight),
            BuildTypes());
        _durationSelector = CreateSelector(
            new Rectangle(ContentX + column + SectionGap, FirstRowTop + RowSpacing, column, Theme.FieldHeight),
            BuildDurations());

        int historyTop = FirstRowTop + (RowSpacing * HistoryRow);

        for (int historyIndex = 0; historyIndex < VisibleHistory; historyIndex++)
        {
            var row = new DataRow
            {
                IsVisible = false,
                Bounds = new Rectangle(
                    ContentX,
                    historyTop + (historyIndex * (HistoryHeight + HistoryGap)),
                    ContentWidth,
                    HistoryHeight)
            };

            _history.Add(row);
            Register(row);
        }

        _applyButton = CreatePrimaryButton(true);
        _cancelButton = CreateSecondaryButton();
        LayOutActionsInRow([_applyButton, _cancelButton]);
        _applyButton.Clicked += OnApplyClicked;
        _cancelButton.Clicked += OnCancelClicked;

        Register(_playerBox);
        Register(_scopeSelector);
        Register(_typeSelector);
        Register(_durationSelector);
        Register(_applyButton);
        Register(_cancelButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.ApplySanctionSubtitle;
    }

    protected override void ApplyTexts()
    {
        _playerBox.Label = TextCatalog.ApplySanctionPlayerLabel;
        _scopeSelector.Label = TextCatalog.ApplySanctionScopeLabel;
        _scopeSelector.Options = BuildScopes();
        _typeSelector.Label = TextCatalog.ApplySanctionTypeLabel;
        _typeSelector.Options = BuildTypes();
        _durationSelector.Label = TextCatalog.ApplySanctionDurationLabel;
        _durationSelector.Options = BuildDurations();
        _applyButton.Title = TextCatalog.ApplySanctionApplyButton;
        _cancelButton.Title = TextCatalog.CommonCancelButton;
    }

    private static Selector CreateSelector(Rectangle bounds, IReadOnlyList<string> options)
    {
        return new Selector
        {
            Options = options,
            Bounds = bounds
        };
    }

    private static IReadOnlyList<string> BuildScopes()
    {
        return [TextCatalog.ApplySanctionScopeChat, TextCatalog.ApplySanctionScopeAccount];
    }

    private static IReadOnlyList<string> BuildTypes()
    {
        return [TextCatalog.ApplySanctionTypeTemporary, TextCatalog.ApplySanctionTypePermanent];
    }

    private static IReadOnlyList<string> BuildDurations()
    {
        return [TextCatalog.ApplySanctionDurationDay, TextCatalog.ApplySanctionDurationWeek];
    }

    private void OnApplyClicked(object? sender, EventArgs e)
    {
        Navigator.ShowConfirm(new ConfirmRequest
        {
            Body = TextCatalog.ApplySanctionConfirmBody,
            PrimaryLabel = TextCatalog.ApplySanctionApplyButton,
            SecondaryLabel = TextCatalog.CommonCancelButton,
            OnConfirm = GoBack
        });
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }

    private void GoBack()
    {
        Navigator.GoBack();
    }
}
