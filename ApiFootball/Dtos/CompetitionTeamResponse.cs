namespace ApiFootball.Dtos;

public record CompetitionTeamResponse(
    List<TeamResponse> Teams,
    CompetitionBaseResponse Competition,
    CompetitionSeasonBaseResponse Season
);

public record TeamBaseResponse(int Id);

public record TeamResponse(
    int Id,
    string Name,
    string ShortName,
    string Crest
) : TeamBaseResponse(Id);