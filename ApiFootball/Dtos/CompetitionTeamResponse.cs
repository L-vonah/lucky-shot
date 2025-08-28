namespace ApiFootball.Dtos;

internal class CompetitionTeamResponse
{
    public List<TeamResponse> Teams { get; init; } = [];
    public CompetitionBaseResponse Competition { get; init; } = null!;
    public CompetitionSeasonBaseResponse Season { get; init; } = null!;
}

internal class TeamBaseResponse
{
    public int Id { get; init; }
}

internal class TeamResponse : TeamBaseResponse
{
    public string Name { get; init; } = null!;
    public string ShortName { get; init; } = null!;
    public string Crest { get; init; } = null!;
}