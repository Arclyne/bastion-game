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

    private static BoardScreen CreateScreen()
    {
        return new BoardScreen(new RecordingNavigator(), null);
    }

    private static MatchView CreateView()
    {
        return CreateScreen().View;
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
