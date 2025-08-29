using ApiFootball.Dtos;
using Refit;

namespace ApiFootball;

internal interface IApiFootball
{
    [Get("/v4/competitions/{acronym}")]
    Task<CompetitionResponse> GetCompetition(string acronym);

    [Get("/v4/competitions/{acronym}/teams?season={year}")]
    Task<CompetitionTeamResponse> GetCompetitionTeams(string acronym, int year);
    
    [Get("/v4/competitions/{acronym}/matches?season={year}")]
    Task<CompetitionMatchResponse> GetCompetitionMatches(string acronym, int year);
}