using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Bastion.Client.Controls;
using Bastion.Client.Rendering;
using Bastion.Domain;

namespace Bastion.Client.Screens.BoardScene;

// Kept apart from the match interface so the board can be built and looked at
// on its own.
public sealed class BoardScreen : IScreen, IWorldScreen
{
    private const int Size = BoardAssetSet.ClassicBoardSize;
    private const int FirstRow = 0;
    private const int LastRow = Size - 1;
    private const int MiddleColumn = Size / 2;
    private const int HorizontalWallColumn = 2;
    private const int HorizontalWallRow = 4;
    private const int VerticalWallColumn = 5;
    private const int VerticalWallRow = 2;

    private static readonly BoardPosition _startingPawn = new BoardPosition(MiddleColumn, FirstRow);
    private static readonly BoardPosition _rivalPawn = new BoardPosition(MiddleColumn, LastRow);

    private readonly INavigator _navigator;
    private readonly BoardRenderer? _renderer;
    private readonly MatchView _view = new MatchView();
    private readonly Board _board;
    private readonly WallPlacementValidator _wallPlacement;

    private Wall _heldWall;
    private BoardPosition _movingPawn = _startingPawn;

    public BoardScreen(INavigator navigator, BoardRenderer? renderer)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        _navigator = navigator;
        _renderer = renderer;
        _board = CreateBoard();
        _wallPlacement = new WallPlacementValidator(_board);
        ShowStartingPosition();
    }

    // What the scene shows. It is filled from here while there is no match
    // service, and later from what the server sends.
    public MatchView View => _view;

    public BoardAction Action { get; private set; }

    // Nothing is previewed until the player says what they are doing, which is
    // what keeps the board clear the rest of the time.
    public void PrepareFor(BoardAction action)
    {
        Action = action;
        _view.PawnPreviews.Clear();
        _view.WallPreview = null;

        if (action == BoardAction.MovePawn)
        {
            ShowReachableCells();
            return;
        }

        if (action == BoardAction.PlaceWall)
        {
            _heldWall = new Wall(default, WallOrientation.Horizontal);
        }
    }

    // CU-21 FA-03: the wall is turned before it is let go of, and it keeps the
    // crossing it was being aimed at.
    public void TurnTheWall()
    {
        if (Action != BoardAction.PlaceWall)
        {
            return;
        }

        _heldWall = _heldWall.Turn();

        if (_view.WallPreview is not null)
        {
            ShowHeldWall();
        }
    }

    // Letting go of the piece puts it on the board and leaves nothing prepared,
    // so every see-through piece goes away with it.
    public void Place(BoardPosition cell)
    {
        if (Action == BoardAction.MovePawn)
        {
            MoveThePawn(cell);
            return;
        }

        if (Action == BoardAction.PlaceWall)
        {
            PlaceTheWall();
        }
    }

    // The crossing comes from the pointer while the game runs, and straight from
    // a test otherwise.
    public void AimAt(BoardPosition crossing)
    {
        if (Action != BoardAction.PlaceWall)
        {
            return;
        }

        _heldWall = _heldWall with { Crossing = crossing };
        ShowHeldWall();
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ReadAction(input);
        ReadWayOut(input);
        AimTheWall(input);
        // Confirming goes first on purpose: the camera clears the mark that says
        // the button was used to turn the board, and reading it afterwards would
        // let the click that ends a drag place a piece.
        Confirm(input);
        _renderer?.Camera.Update(input);
    }

    public void DrawWorld(BoardRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);

        renderer.Draw(_view);
    }

    // The scene is only the board, so there is nothing over the world the game
    // has already drawn.
    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);
    }

    private static Board CreateBoard()
    {
        Wall[] walls =
        [
            new Wall(new BoardPosition(HorizontalWallColumn, HorizontalWallRow), WallOrientation.Horizontal),
            new Wall(new BoardPosition(VerticalWallColumn, VerticalWallRow), WallOrientation.Vertical),
        ];

        return new Board(Size, walls, [_startingPawn, _rivalPawn]);
    }

    private void ReadAction(InputState input)
    {
        if (input.IsKeyNewlyPressed(Keys.Q))
        {
            PrepareFor(BoardAction.MovePawn);
        }

        if (input.IsKeyNewlyPressed(Keys.E))
        {
            PrepareFor(BoardAction.PlaceWall);
        }

        if (input.IsKeyNewlyPressed(Keys.R))
        {
            TurnTheWall();
        }
    }

    // Escape drops what is being prepared first, and only leaves the scene when
    // there is nothing left to drop and somewhere to go back to.
    private void ReadWayOut(InputState input)
    {
        if (!input.IsKeyNewlyPressed(Keys.Escape))
        {
            return;
        }

        if (Action != BoardAction.None)
        {
            PrepareFor(BoardAction.None);
            return;
        }

        if (_navigator.CanGoBack)
        {
            _navigator.GoBack();
        }
    }

    // A stand-in while there is no match service. The two walls also show that a
    // wall lands on the crossing its anchor marks.
    private void ShowStartingPosition()
    {
        _view.Pawns.Add(new PawnMarker(_startingPawn, Theme.FirstPlayer));
        _view.Pawns.Add(new PawnMarker(_rivalPawn, Theme.SecondPlayer));

        foreach (Wall wall in _board.Walls)
        {
            _view.Walls.Add(new WallMarker(wall.Crossing, wall.Orientation, Theme.FirstPlayer));
        }
    }

    // Answering from the rules shows the cells at once, while the move still
    // travels to the server, which decides (CU-20 step 3, RN-01).
    private void ShowReachableCells()
    {
        IReadOnlyList<BoardPosition> cells = new PawnMoveFinder(_board).GetMoves(_movingPawn);

        foreach (BoardPosition cell in cells)
        {
            _view.PawnPreviews.Add(new PawnMarker(cell, Theme.Preview));
        }
    }

    // The held wall rides on the crossing the pointer is over, and nowhere else:
    // off the board there is no crossing to lay it on, so it is not drawn.
    private void AimTheWall(InputState input)
    {
        if (Action != BoardAction.PlaceWall || _renderer is null)
        {
            return;
        }

        BoardPosition? crossing = _renderer.FindCrossing(input.MousePosition);
        if (crossing is null)
        {
            _view.WallPreview = null;
            return;
        }

        AimAt(crossing.Value);
    }

    // The camera owns dragging with the same button, so only a click that did not
    // turn the board counts as letting a piece go.
    private void Confirm(InputState input)
    {
        if (Action == BoardAction.None || !input.HasClicked || _renderer is null)
        {
            return;
        }

        if (_renderer.Camera.HasTurnedWhilePressed)
        {
            return;
        }

        BoardPosition? cell = _renderer.FindCell(input.MousePosition);
        if (cell is not null)
        {
            Place(cell.Value);
        }
    }

    // Only a cell the rules allow, which is one of the cells being previewed.
    private void MoveThePawn(BoardPosition cell)
    {
        if (!_view.PawnPreviews.Exists(preview => preview.Cell == cell))
        {
            return;
        }

        _board.MovePawn(_movingPawn, cell);
        _view.Pawns.RemoveAll(pawn => pawn.Cell == _movingPawn);
        _movingPawn = cell;
        _view.Pawns.Add(new PawnMarker(cell, Theme.FirstPlayer));
        PrepareFor(BoardAction.None);
    }

    // CU-21 RN-04: a wall that overlaps or crosses another one is not let go of.
    private void PlaceTheWall()
    {
        if (_view.WallPreview is null || !IsHeldWallAllowed())
        {
            return;
        }

        _board.Place(_heldWall);
        _view.Walls.Add(new WallMarker(_heldWall.Crossing, _heldWall.Orientation, Theme.FirstPlayer));
        PrepareFor(BoardAction.None);
    }

    // CU-21 step 3: the wall already answers while it is being aimed, so the
    // player sees that a crossing is turned down before letting the wall go.
    private void ShowHeldWall()
    {
        Color tint = IsHeldWallAllowed() ? Theme.Preview : Theme.PreviewBlocked;

        _view.WallPreview = new WallMarker(_heldWall.Crossing, _heldWall.Orientation, tint);
    }

    private bool IsHeldWallAllowed()
    {
        return _wallPlacement.Check(_heldWall) == WallPlacementResult.Allowed;
    }
}
