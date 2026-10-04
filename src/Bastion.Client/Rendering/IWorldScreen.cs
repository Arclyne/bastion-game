namespace Bastion.Client.Rendering;

// A screen with a 3D world behind its interface. The game draws the world before
// the sprite batch opens, and the screen then draws its interface on top.
public interface IWorldScreen
{
    void DrawWorld(BoardRenderer renderer);
}
