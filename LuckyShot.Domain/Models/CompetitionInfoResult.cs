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
    string Logo
);

public record MatchesInfoResult(
    int ExternalId,
    DateTime MatchDate,
    DateTime LastUpdatedDate,
    int Round,
    MatchStatus Status,
    MatchResult Result,
    int? HomeTeamId,
    int? AwayTeamId,
    int? HomeTeamFullTimeScore,
    int? AwayTeamFullTimeScore,
    int? HomeTeamHalfTimeScore,
    int? AwayTeamHalfTimeScore
);

public record CompetitionMatchesInfoResult(
    int SeasonExternalId,
    MatchesInfoResult[] Matches
);