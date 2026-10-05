using System;
using Bastion.Client.Controls;

namespace Bastion.Client.Screens;

public abstract class MessageScreen : IScreen
{
    protected MessageScreen(DialogTone tone)
    {
        Dialog = new MessageDialog(tone);
    }

    public MessageDialog Dialog { get; }

    public void Show(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        Dialog.Body = body;
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        Dialog.Update(input);
    }

    public void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        Dialog.Draw(canvas);
    }
}
