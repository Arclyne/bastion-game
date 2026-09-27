using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

// Confirms that an operation finished. It is the only tone that does not use
// the accent color, so success is not read as another failure.
public sealed class MessageSuccessScreen : MessageScreen
{
    public MessageSuccessScreen()
        : base(DialogTone.Success)
    {
        Dialog.Title = TextCatalog.DialogSuccessTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }
}
