namespace ApiFootball.Dtos;

public record CompetitionSummaryResponse(
    CompetitionResponse Competition,
    CompetitionSeasonResponse Season,
    List<TeamResponse> Teams
);

public record CompetitionBaseResponse(int Id);
public record CompetitionResponse(
    int Id,
    string Name,
    string Code,
    string Emblem
) : CompetitionBaseResponse(Id);

public record CompetitionSeasonBaseResponse(int Id);
public record CompetitionSeasonResponse(
    int Id,
    DateTime? StartDate,
    DateTime? EndDate,
    int? CurrentMatchday
) : CompetitionSeasonBaseResponse(Id);