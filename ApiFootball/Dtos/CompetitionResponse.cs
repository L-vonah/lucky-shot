namespace ApiFootball.Dtos;

internal class CompetitionBaseResponse
{
    public int Id { get; init; }
}

internal class CompetitionResponse : CompetitionBaseResponse
{
    public string Name { get; init; } = null!;
    public string Code { get; init; } = null!;
    public string Emblem { get; init; } = null!;
    public CompetitionCurrentSeasonResponse CurrentSeason { get; init; } = null!;
}

internal class CompetitionSeasonBaseResponse
{
    public int Id { get; init; }
}

internal class CompetitionCurrentSeasonResponse : CompetitionSeasonBaseResponse
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int? CurrentMatchday { get; init; }
}