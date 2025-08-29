namespace ApiFootball.Dtos;

public record CompetitionMatchResponse(
    List<MatchResponse> Matches,
    CompetitionBaseResponse Competition
);

public record MatchResponse(
    int Id,
    DateTime UtcDate,
    DateTime LastUpdated,
    int Matchday,
    string Status,
    TeamBaseResponse HomeTeam,
    TeamBaseResponse AwayTeam,
    ScoreResponse Score
);

public record ScoreResponse(
    ScoreDetailResponse FullTime,
    ScoreDetailResponse HalfTime
);

public record ScoreDetailResponse(
    int? Home,
    int? Away
);