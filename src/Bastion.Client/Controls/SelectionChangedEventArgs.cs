using System;

namespace Bastion.Client.Controls;

public sealed class SelectionChangedEventArgs : EventArgs
{
    public required int SelectedIndex { get; init; }
}
