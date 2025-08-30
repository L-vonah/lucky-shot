using ApiFootball;
using ApiFootball.Dtos;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.ExternalServices.Mappers;

namespace LuckyShot.Infrastructure.ExternalServices;

public class ApiFootballService(
    IApiFootball apiFootball
) : ICompetitionInfoProvider
{
    public async Task<CompetitionSummaryResult> FetchCompetitionSummary(CompetitionCategory category, int year)
    {
        var acronym = category.ToApiFootballCompetitionAcronym();
        var response = await apiFootball.GetCompetitionSummary(acronym, year);
        return response.ToDomainCompetitionSummaryResult();
    }

    public async Task<IEnumerable<CompetitionMatchesInfoResult>> FetchCompetitionMatchesInformation(CompetitionCategory category, int year)
    {
        var acronym = category.ToApiFootballCompetitionAcronym();
        var response = await apiFootball.GetCompetitionMatches(acronym, year);
        return response.Matches.Select(m => m.ToDomainCompetitionMatchesInfoResult());
    }
}