namespace ApiFootball.Dtos;

public record CompetitionBaseResponse(int Id);

public record CompetitionResponse(
    int Id,
    string Name,
    string Code,
    string Emblem,
    CompetitionCurrentSeasonResponse? CurrentSeason
) : CompetitionBaseResponse(Id);

public record CompetitionSeasonBaseResponse(int? Id);

public record CompetitionCurrentSeasonResponse(
    int? Id,
    DateTime? StartDate,
    DateTime? EndDate,
    int? CurrentMatchday
) : CompetitionSeasonBaseResponse(Id);