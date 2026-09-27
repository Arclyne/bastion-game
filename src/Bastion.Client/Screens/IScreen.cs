using Bastion.Client.Controls;

namespace Bastion.Client.Screens;

// Anything the navigator can put on screen: a form, a list or a dialog.
public interface IScreen
{
    void Update(InputState input);

    void Draw(Canvas canvas);
}
