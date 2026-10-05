using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

public sealed class MessageSuccessScreen : MessageScreen
{
    public MessageSuccessScreen()
        : base(DialogTone.Success)
    {
        Dialog.Title = TextCatalog.DialogSuccessTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }
}
