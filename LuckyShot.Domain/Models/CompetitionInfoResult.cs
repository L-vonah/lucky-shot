using LuckyShot.Domain.Entities;

namespace LuckyShot.Domain.Models;

public record CompetitionInfoResult(
    int ExternalId,
    string Name,
    string Code,
    string Logo,
    int? CurrentSeasonId,
    DateTime? CurrentSeasonStartDate,
    DateTime? CurrentSeasonEndDate,
    int? CurrentSeasonRound
);

public record CompetitionTeamsInfoResult(
    int ExternalId,
    string Name,
    string ShortName,
    string Logo
);

public record CompetitionMatchesInfoResult(
    int ExternalId,
    DateTime MatchDate,
    DateTime LastUpdatedDate,
    int Matchday,
    MatchStatus Status,
    int HomeTeamId,
    int AwayTeamId,
    int? HomeTeamFullTimeScore,
    int? AwayTeamFullTimeScore,
    int? HomeTeamHalfTimeScore,
    int? AwayTeamHalfTimeScore
);