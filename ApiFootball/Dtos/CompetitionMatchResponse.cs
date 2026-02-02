namespace ApiFootball.Dtos;

public record CompetitionMatchResponse(
    List<MatchResponse> Matches
);

public record MatchResponse(
    int Id,
    DateTimeOffset UtcDate,
    DateTimeOffset LastUpdated,
    int? Matchday,
    string Status,
    TeamBaseResponse HomeTeam,
    TeamBaseResponse AwayTeam,
    ScoreResponse Score,
    CompetitionSeasonBaseResponse Season
);

public record ScoreResponse(
    string? Winner,
    ScoreDetailResponse FullTime,
    ScoreDetailResponse HalfTime
);

public record ScoreDetailResponse(
    int? Home,
    int? Away
);