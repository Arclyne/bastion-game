namespace Bastion.Contracts.Leaderboard;

public static class LeaderboardRules
{
    // A player needs five ranked matches in a mode to appear in its ranking (CU-35 RN-01).
    public const int MinimumMatches = 5;
    public const int PageSize = 8;

    // Default ranking shown when the screen opens: the classic 9x9 mode (CU-35 RN-03).
    public const string ClassicModeCode = "CLASSIC";
    public const string FourPlayersModeCode = "FOUR_PLAYERS";
    public const string QuickModeCode = "QUICK";
}
