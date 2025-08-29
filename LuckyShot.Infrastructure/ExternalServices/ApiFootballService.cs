using ApiFootball;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.ExternalServices.Mappers;

namespace LuckyShot.Infrastructure.ExternalServices;

public class ApiFootballService(
    IApiFootball apiFootball
) : ICompetitionInfoProvider
{
    public async Task<CompetitionInfoResult> FetchCompetitionInformation(CompetitionCategory category)
    {
        var acronym = category.ToApiFootballCompetitionAcronym();
        var response = await apiFootball.GetCompetition(acronym);
        return response.ToDomainCompetitionInfoResult();
    }

    public async Task<IEnumerable<CompetitionTeamsInfoResult>> FetchCompetitionTeamsInformation(CompetitionCategory category, int year)
    {
        var acronym = category.ToApiFootballCompetitionAcronym();
        var response = await apiFootball.GetCompetitionTeams(acronym, year);
        return response.Teams.Select(t => t.ToDomainCompetitionTeamsInfoResult());
    }

    public async Task<IEnumerable<CompetitionMatchesInfoResult>> FetchCompetitionMatchesInformation(CompetitionCategory category, int year)
    {
        var acronym = category.ToApiFootballCompetitionAcronym();
        var response = await apiFootball.GetCompetitionMatches(acronym, year);
        return response.Matches.Select(m => m.ToDomainCompetitionMatchesInfoResult());
    }
}