using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.TutorialIndex;

// CU-41 main flow step 1. Seven lessons; opening a lesson itself needs a
// board, which does not exist yet, so each row is informational until then.
// Which lessons are done or in progress comes from the server, so the
// progress line and the state tags stay empty until it answers.
public sealed class TutorialIndexScreen : FormScreen
{
    private const int LessonCount = 7;

    private const int ProgressHeight = 20;
    private const int SectionGap = 14;
    private const int RowHeight = 40;
    private const int RowGap = 6;

    private const int RowsTop = ProgressHeight + SectionGap;
    private const int ContentHeight = RowsTop + (LessonCount * (RowHeight + RowGap)) - RowGap;
    private const int CardHeight = Theme.CardPadding + ContentHeight + Theme.CardPadding;

    private readonly TextLine _progress;
    private readonly List<DataRow> _lessons = [];
    private readonly Button _practiceButton;
    private readonly Button _exitButton;

    public TutorialIndexScreen(INavigator navigator)
        : base(navigator, WideCardWidth, CardHeight)
    {
        int top = Card.Y + Theme.CardPadding;

        _progress = new TextLine
        {
            Style = TextLineStyle.Label,
            Bounds = new Rectangle(ContentX, top, ContentWidth, ProgressHeight)
        };

        for (int lessonIndex = 0; lessonIndex < LessonCount; lessonIndex++)
        {
            int rowTop = top + RowsTop + (lessonIndex * (RowHeight + RowGap));
            var row = new DataRow { Bounds = new Rectangle(ContentX, rowTop, ContentWidth, RowHeight) };
            _lessons.Add(row);
            Register(row);
        }

        _practiceButton = CreatePrimaryButton(true);
        _exitButton = CreateSecondaryButton();
        _practiceButton.Clicked += OnPracticeClicked;
        _exitButton.Clicked += OnExitClicked;

        Register(_progress);
        Register(_practiceButton);
        Register(_exitButton);

        ApplyTexts();
    }

    protected override string GetSubtitle()
    {
        return TextCatalog.TutorialIndexSubtitle;
    }

    protected override void ApplyTexts()
    {
        string[] names =
        [
            TextCatalog.LessonBoardAndGoal,
            TextCatalog.LessonMovePawn,
            TextCatalog.LessonJumpOpponent,
            TextCatalog.LessonPlaceWalls,
            TextCatalog.LessonWallsNoTrap,
            TextCatalog.LessonClock,
            TextCatalog.LessonFourPlayers
        ];

        for (int lessonIndex = 0; lessonIndex < _lessons.Count; lessonIndex++)
        {
            _lessons[lessonIndex].Title = names[lessonIndex];
        }

        _practiceButton.Title = TextCatalog.TutorialIndexPracticeButton;
        _exitButton.Title = TextCatalog.TutorialIndexExitButton;
    }

    private void OnPracticeClicked(object? sender, EventArgs e)
    {
        Navigator.GoTo(ScreenId.AIDifficulty);
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Navigator.GoBack();
    }
}
