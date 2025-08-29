namespace ApiFootball.Dtos;

internal class CompetitionMatchResponse
{
    public List<MatchResponse> Matches { get; init; } = [];
    public CompetitionBaseResponse Competition { get; init; } = null!;
}

internal class MatchResponse
{
    public int Id { get; init; }
    public DateTime UtcDate { get; init; }
    public DateTime LastUpdated { get; init; }
    public int Matchday { get; init; }
    public string Status { get; init; } = null!;
    public TeamBaseResponse HomeTeam { get; init; } = null!;
    public TeamBaseResponse AwayTeam { get; init; } = null!;
    public ScoreResponse Score { get; init; } = null!;
}

internal class ScoreResponse
{
    public ScoreDetailResponse FullTime { get; init; } = null!;
    public ScoreDetailResponse HalfTime { get; init; } = null!;
}

internal class ScoreDetailResponse
{
    public int? Home { get; init; }
    public int? Away { get; init; }
}