using LuckyShot.Domain.Entities;

namespace LuckyShot.Domain.Models;

public record CompetitionSummaryResult(
    CompetitionInfoResult Competition,
    CompetitionSeasonInfoResult Season,
    CompetitionTeamInfoResult[] Teams
);

public record CompetitionInfoResult(
    int ExternalId,
    string Name,
    string Code,
    string Logo
);

public record CompetitionSeasonInfoResult(
    int ExternalId,
    DateTime? StartDate,
    DateTime? EndDate,
    int? CurrentRound
);

public record CompetitionTeamInfoResult(
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