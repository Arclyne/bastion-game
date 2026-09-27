using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

// Reports something the player can still fix: a malformed field, a password
// that breaks the policy, a nickname already taken.
public sealed class MessageWarningScreen : MessageScreen
{
    public MessageWarningScreen()
        : base(DialogTone.Warning)
    {
        Dialog.Title = TextCatalog.DialogWarningTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }

    // CU-02 FA-10 adds a way out towards the sign in screen.
    public void ShowWithSignIn(string body)
    {
        Show(body);
        Dialog.SecondaryLabel = TextCatalog.DialogSignInButton;
    }
}
