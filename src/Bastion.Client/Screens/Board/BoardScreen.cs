using System;
using Microsoft.Xna.Framework.Input;
using Bastion.Client.Controls;
using Bastion.Client.Rendering;

namespace Bastion.Client.Screens.Board;

// Kept apart from the match interface so the board can be built and looked at
// on its own.
public sealed class BoardScreen : IScreen, IWorldScreen
{
    private readonly INavigator _navigator;
    private readonly BoardRenderer? _renderer;
    private readonly MatchView _view = new MatchView();

    public BoardScreen(INavigator navigator, BoardRenderer? renderer)
    {
        ArgumentNullException.ThrowIfNull(navigator);

        _navigator = navigator;
        _renderer = renderer;
        ShowStartingPosition();
    }

    // What the scene shows. It is filled from here while there is no match
    // service, and later from what the server sends.
    public MatchView View => _view;

    // A stand-in while there is no match service. The two walls also show that a
    // wall lands on the crossing its anchor marks.
    private void ShowStartingPosition()
    {
        const int FirstRow = 0;
        const int LastRow = BoardAssetSet.ClassicBoardSize - 1;
        const int MiddleColumn = BoardAssetSet.ClassicBoardSize / 2;
        const int HorizontalWallColumn = 2;
        const int HorizontalWallRow = 4;
        const int VerticalWallColumn = 5;
        const int VerticalWallRow = 2;

        _view.Pawns.Add(new PawnMarker(new BoardSlot(MiddleColumn, FirstRow), Theme.FirstPlayer));
        _view.Pawns.Add(new PawnMarker(new BoardSlot(MiddleColumn, LastRow), Theme.SecondPlayer));
        _view.Walls.Add(new WallMarker(
            new BoardSlot(HorizontalWallColumn, HorizontalWallRow),
            WallOrientation.Horizontal,
            Theme.FirstPlayer));
        _view.Walls.Add(new WallMarker(
            new BoardSlot(VerticalWallColumn, VerticalWallRow),
            WallOrientation.Vertical,
            Theme.SecondPlayer));
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Opened with --board there is nowhere to go back to, and going back would
        // rebuild the scene and throw away the angle the player turned it to.
        if (input.IsKeyNewlyPressed(Keys.Escape) && _navigator.CanGoBack)
        {
            _navigator.GoBack();
            return;
        }

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
}
