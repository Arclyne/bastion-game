namespace Bastion.Server.Data.Entities;

public class GameMode
{
    public byte GameModeId { get; set; }

    public string Code { get; set; } = string.Empty;

    public byte BoardSize { get; set; }

    public byte PlayerCount { get; set; }

    public byte WallsPerPlayer { get; set; }

    public bool IsActive { get; set; }
}
