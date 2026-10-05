using System;
using Bastion.Client.Localization;

namespace Bastion.Client.Screens.Messages;

public sealed class MessageErrorScreen : MessageScreen
{
    public MessageErrorScreen()
        : base(DialogTone.Error)
    {
        Dialog.Title = TextCatalog.DialogErrorTitle;
        Dialog.PrimaryLabel = TextCatalog.DialogAcceptButton;
    }

    public void ShowRetryable(string body, string incident)
    {
        ArgumentNullException.ThrowIfNull(incident);

        Show(body);
        Dialog.Detail = incident;
        Dialog.SecondaryLabel = TextCatalog.DialogRetryButton;
    }
}
