using System.Collections.Generic;
using System.Linq;
using Xunit;
using Bastion.Client.Rendering;
using Bastion.Client.Screens.BoardScene;
using Bastion.Client.Tests.Navigation;
using Bastion.Domain;

namespace Bastion.Client.Tests;

// The scene is built without a renderer, which is what happens when the models
// cannot be loaded: it must still stand up and hold what it would draw.
public sealed class TestBoardScreen
{
    private const int BoardSize = 9;
    private const int MiddleColumn = BoardSize / 2;
    private const int LastRow = BoardSize - 1;
    private const int CrossingCount = BoardSize - 1;
    private const int PlayerCount = 2;
    private const int OrientationCount = 2;

    // The pawn that moves starts on the middle cell of the bottom row, so three
    // of its four neighbours are on the board.
    private const int ReachableFromTheBackRow = 3;

    private static readonly BoardPosition _aimedCrossing = new BoardPosition(3, 5);

    // The horizontal wall of the starting position already stands here.
    private static readonly BoardPosition _takenCrossing = new BoardPosition(2, 4);
    private static readonly BoardPosition _unreachableCell = new BoardPosition(0, LastRow);

    private static BoardScreen CreateScreen()
    {
        return new BoardScreen(new RecordingNavigator(), null);
    }

    private static MatchView CreateView()
    {
        return CreateScreen().View;
    }

    private static BoardScreen CreateScreenAimedAt(BoardPosition crossing)
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.PlaceWall);
        screen.AimAt(crossing);

        return screen;
    }

    private static MatchView CreateViewAimedAt(BoardPosition crossing)
    {
        return CreateScreenAimedAt(crossing).View;
    }

    private static MatchView CreateViewPreparedFor(BoardAction action)
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(action);

        return screen.View;
    }

    [Fact]
    public void View_ANewScene_HasOnePawnForEachPlayer()
    {
        int pawns = CreateView().Pawns.Count;

        Assert.Equal(PlayerCount, pawns);
    }

    [Fact]
    public void View_ANewScene_PutsThePawnsOnOppositeBackRows()
    {
        MatchView view = CreateView();

        int[] rows = view.Pawns.Select(pawn => pawn.Cell.Row).OrderBy(row => row).ToArray();

        Assert.Equal([0, LastRow], rows);
    }

    [Fact]
    public void View_ANewScene_PutsBothPawnsOnTheMiddleColumn()
    {
        MatchView view = CreateView();

        bool isEveryPawnOnTheMiddleColumn = view.Pawns.All(pawn => pawn.Cell.Column == MiddleColumn);

        Assert.True(isEveryPawnOnTheMiddleColumn);
    }

    [Fact]
    public void View_ANewScene_ShowsOneWallOfEachOrientation()
    {
        MatchView view = CreateView();

        int orientations = view.Walls.Select(wall => wall.Orientation).Distinct().Count();

        Assert.Equal(OrientationCount, orientations);
    }

    [Fact]
    public void View_ANewScene_KeepsEveryWallOnACrossingOfTheBoard()
    {
        MatchView view = CreateView();

        bool isEveryWallOnACrossing = view.Walls.All(IsOnACrossing);

        Assert.True(isEveryWallOnACrossing);
    }

    // Nothing is previewed until the player says what they are doing.
    [Fact]
    public void Action_ANewScene_IsNone()
    {
        BoardAction action = CreateScreen().Action;

        Assert.Equal(BoardAction.None, action);
    }

    [Fact]
    public void View_ANewScene_PreviewsNoPawn()
    {
        int previews = CreateView().PawnPreviews.Count;

        Assert.Equal(0, previews);
    }

    [Fact]
    public void View_ANewScene_PreviewsNoWall()
    {
        WallMarker? preview = CreateView().WallPreview;

        Assert.Null(preview);
    }

    // CU-20 step 3: the cells the pawn can reach, and only those.
    [Fact]
    public void View_PreparedToMove_PreviewsEveryCellTheRulesAllow()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.MovePawn);

        int previews = view.PawnPreviews.Count;

        Assert.Equal(ReachableFromTheBackRow, previews);
    }

    [Fact]
    public void View_PreparedToMove_NeverPreviewsTheCellThePawnStandsOn()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.MovePawn);
        IEnumerable<BoardPosition> taken = view.Pawns.Select(pawn => pawn.Cell).ToList();

        bool isPreviewingATakenCell = view.PawnPreviews.Any(preview => taken.Contains(preview.Cell));

        Assert.False(isPreviewingATakenCell);
    }

    [Fact]
    public void View_PreparedToMove_PreviewsNoWall()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.MovePawn);

        Assert.Null(view.WallPreview);
    }

    // The wall rides on the crossing the pointer is over, so until it is aimed
    // there is no crossing to lay it on and nothing is drawn.
    [Fact]
    public void View_PreparedToPlaceAWallAndNotAimedYet_PreviewsNoWall()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.PlaceWall);

        Assert.Null(view.WallPreview);
    }

    [Fact]
    public void View_PreparedToPlaceAWall_PreviewsNoPawn()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.PlaceWall);

        int previews = view.PawnPreviews.Count;

        Assert.Equal(0, previews);
    }

    // Dropping what was being prepared clears the board again.
    [Fact]
    public void View_PreparedToMoveAndThenToNothing_PreviewsNoPawn()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.PrepareFor(BoardAction.None);

        Assert.Empty(screen.View.PawnPreviews);
    }

    [Fact]
    public void View_PreparedToPlaceAWallAndThenToMove_PreviewsNoWall()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.PlaceWall);

        screen.PrepareFor(BoardAction.MovePawn);

        Assert.Null(screen.View.WallPreview);
    }

    // CU-21 FA-03: the wall is turned before it is let go of. It comes out of the
    // inventory the way the model is modelled, along the columns.
    [Fact]
    public void View_AWallJustAimed_LiesAlongTheColumns()
    {
        MatchView view = CreateViewAimedAt(_aimedCrossing);

        Assert.Equal(WallOrientation.Horizontal, view.WallPreview?.Orientation);
    }

    [Fact]
    public void View_AWallTurnedOnce_LiesAlongTheRows()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);

        screen.TurnTheWall();

        Assert.Equal(WallOrientation.Vertical, screen.View.WallPreview?.Orientation);
    }

    [Fact]
    public void View_AWallTurnedTwice_LiesAlongTheColumnsAgain()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);

        screen.TurnTheWall();
        screen.TurnTheWall();

        Assert.Equal(WallOrientation.Horizontal, screen.View.WallPreview?.Orientation);
    }

    [Fact]
    public void View_AWallTurned_StaysOnTheCrossingItWasAimedAt()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);

        screen.TurnTheWall();

        Assert.Equal(_aimedCrossing, screen.View.WallPreview?.Groove);
    }

    [Fact]
    public void View_AWallTakenAfterTurningTheLastOne_LiesAlongTheColumns()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);
        screen.TurnTheWall();

        screen.PrepareFor(BoardAction.PlaceWall);
        screen.AimAt(_aimedCrossing);

        Assert.Equal(WallOrientation.Horizontal, screen.View.WallPreview?.Orientation);
    }

    // Nothing is held while a move is being made, so there is nothing to turn.
    [Fact]
    public void View_TurnedWhileMoving_PreviewsNoWall()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.TurnTheWall();

        Assert.Null(screen.View.WallPreview);
    }

    // Letting the pawn go leaves it on the board and takes every see-through
    // piece away with it.
    [Fact]
    public void View_APawnLetGoOnAReachableCell_StandsOnThatCell()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);
        BoardPosition cell = screen.View.PawnPreviews[0].Cell;

        screen.Place(cell);

        Assert.Contains(screen.View.Pawns, pawn => pawn.Cell == cell);
    }

    [Fact]
    public void View_APawnLetGo_PreviewsNoPawnAnyMore()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.Place(screen.View.PawnPreviews[0].Cell);

        Assert.Empty(screen.View.PawnPreviews);
    }

    [Fact]
    public void Action_APawnLetGo_IsNoneAgain()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.Place(screen.View.PawnPreviews[0].Cell);

        Assert.Equal(BoardAction.None, screen.Action);
    }

    [Fact]
    public void View_APawnLetGo_KeepsTheSameNumberOfPawns()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.Place(screen.View.PawnPreviews[0].Cell);

        Assert.Equal(PlayerCount, screen.View.Pawns.Count);
    }

    // A cell the rules do not allow is not a place to let the pawn go.
    [Fact]
    public void Action_APawnLetGoOnACellOutOfReach_IsStillMoving()
    {
        BoardScreen screen = CreateScreen();
        screen.PrepareFor(BoardAction.MovePawn);

        screen.Place(_unreachableCell);

        Assert.Equal(BoardAction.MovePawn, screen.Action);
    }

    [Fact]
    public void View_AWallLetGoOnAFreeCrossing_StandsOnTheBoard()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);
        int walls = screen.View.Walls.Count;

        screen.Place(_aimedCrossing);

        Assert.Equal(walls + 1, screen.View.Walls.Count);
    }

    [Fact]
    public void View_AWallLetGo_PreviewsNoWallAnyMore()
    {
        BoardScreen screen = CreateScreenAimedAt(_aimedCrossing);

        screen.Place(_aimedCrossing);

        Assert.Null(screen.View.WallPreview);
    }

    // CU-21 RN-04: a wall that overlaps another one is not let go of.
    [Fact]
    public void View_AWallLetGoWhereAnotherOneStands_StaysOffTheBoard()
    {
        BoardScreen screen = CreateScreenAimedAt(_takenCrossing);
        int walls = screen.View.Walls.Count;

        screen.Place(_takenCrossing);

        Assert.Equal(walls, screen.View.Walls.Count);
    }

    // What is about to be placed is drawn see-through, which is what tells it
    // apart from a piece already on the board.
    [Fact]
    public void View_APreviewedPawn_IsNotSolid()
    {
        MatchView view = CreateViewPreparedFor(BoardAction.MovePawn);

        byte? alpha = view.PawnPreviews.FirstOrDefault()?.Tint.A;

        Assert.True(alpha < byte.MaxValue);
    }

    [Fact]
    public void View_APawnOnTheBoard_IsSolid()
    {
        MatchView view = CreateView();

        bool isSolid = view.Pawns.TrueForAll(pawn => pawn.Tint.A == byte.MaxValue);

        Assert.True(isSolid);
    }

    private static bool IsOnACrossing(WallMarker wall)
    {
        return wall.Groove.Column >= 0 && wall.Groove.Column < CrossingCount
            && wall.Groove.Row >= 0 && wall.Groove.Row < CrossingCount;
    }
}
