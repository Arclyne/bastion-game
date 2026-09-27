using System;

namespace Bastion.Server.Data.Entities;

public class ModeStatistic
{
    public int AccountId { get; set; }

    public byte GameModeId { get; set; }

    public short EloRating { get; set; }

    public DateTime? LastMatchAt { get; set; }

    public int MatchesPlayed { get; set; }

    public int MatchesWon { get; set; }

    public int MatchesLost { get; set; }

    public int Abandonments { get; set; }

    public Account? Account { get; set; }

    public GameMode? GameMode { get; set; }
}
