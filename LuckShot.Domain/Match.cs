namespace LuckShot.Domain;

public class Match(
    DateTime date,
    MatchStatus status,
    int matchRound,
    int homeTeamId,
    int awayTeamId,
    int externalId)
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = date;
    public MatchStatus Status { get; set; } = status;
    public int MatchRound { get; set; } = matchRound;
    public int HomeTeamId { get; set; } = homeTeamId;
    public Team? HomeTeam { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamId { get; set; } = awayTeamId;
    public Team? AwayTeam { get; set; }
    public int AwayTeamScore { get; set; }
    public int SeasonId { get; set; }
    public Season? Season { get; set; }
    public int ExternalId { get; set; } = externalId;
}

public enum MatchStatus
{
    Scheduled,
    Live,
    InPlay,
    Paused,
    Finished,
    Postponed,
    Suspended,
    Cancelled
}