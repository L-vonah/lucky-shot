using ApiFootball.Dtos;
using Refit;

namespace ApiFootball;

public interface IApiFootball
{
    [Get("/v4/competitions/{acronym}/teams?season={year}")]
    Task<CompetitionSummaryResponse> GetCompetitionSummary(string acronym, int year);
    
    [Get("/v4/competitions/{acronym}/matches?season={year}")]
    Task<CompetitionMatchResponse> GetCompetitionMatches(string acronym, int year);
}