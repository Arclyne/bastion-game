using System;
using System.Collections.Generic;
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
    private const int HeldWallColumn = 1;
    private const int HeldWallRow = 1;

    private static readonly BoardPosition _movingPawn = new BoardPosition(MiddleColumn, FirstRow);
    private static readonly BoardPosition _rivalPawn = new BoardPosition(MiddleColumn, LastRow);

    private readonly INavigator _navigator;
    private readonly BoardRenderer? _renderer;
    private readonly MatchView _view = new MatchView();
    private readonly Board _board;

    public BoardScreen(INavigator navigator, BoardRenderer? renderer)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        _navigator = navigator;
        _renderer = renderer;
        _board = CreateBoard();
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
            ShowHeldWall();
        }
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        ReadAction(input);
        ReadWayOut(input);
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

        return new Board(Size, walls, [_movingPawn, _rivalPawn]);
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
        _view.Pawns.Add(new PawnMarker(_movingPawn, Theme.FirstPlayer));
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

    // Until there is a pointer over the board, the held wall sits on a crossing
    // picked here.
    private void ShowHeldWall()
    {
        var crossing = new BoardPosition(HeldWallColumn, HeldWallRow);

        _view.WallPreview = new WallMarker(crossing, WallOrientation.Vertical, Theme.Preview);
    }
}
