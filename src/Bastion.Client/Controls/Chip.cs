namespace Bastion.Client.Controls;

public sealed class Chip
{
    public string Text { get; set; } = string.Empty;

    public bool IsSelected { get; set; }

    public bool IsDashed { get; set; }

    public bool IsMuted { get; set; }
}
