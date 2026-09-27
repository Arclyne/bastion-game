namespace Bastion.Server.Data.Repositories;

public sealed record LeaderboardRow(string Nickname, short EloRating, int MatchesPlayed, int MatchesWon);
