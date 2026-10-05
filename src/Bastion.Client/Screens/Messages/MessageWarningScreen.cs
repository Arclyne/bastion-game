using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

public sealed class MessageWarningScreen : MessageScreen
{
    public MessageWarningScreen()
        : base(DialogTone.Warning)
    {
        Dialog.Title = TextCatalog.DialogWarningTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }

    public void ShowWithSignIn(string body)
    {
        Show(body);
        Dialog.SecondaryLabel = TextCatalog.DialogSignInButton;
    }
}
