using Bastion.Client.Controls;

namespace Bastion.Client.Screens;

public interface IScreen
{
    void Update(InputState input);

    void Draw(Canvas canvas);
}
