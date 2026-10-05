using System;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

public sealed class MessageConfirmScreen : MessageScreen
{
    public MessageConfirmScreen()
        : base(DialogTone.Confirm)
    {
        Dialog.Title = TextCatalog.DialogConfirmTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogConfirmButton;
        Dialog.SecondaryLabel = TextCatalog.DialogCancelButton;
    }

    public void ShowWithLabels(string body, string primaryLabel, string secondaryLabel)
    {
        ArgumentNullException.ThrowIfNull(primaryLabel);
        ArgumentNullException.ThrowIfNull(secondaryLabel);

        Show(body);
        Dialog.PrimaryLabel = primaryLabel;
        Dialog.SecondaryLabel = secondaryLabel;
    }
}
