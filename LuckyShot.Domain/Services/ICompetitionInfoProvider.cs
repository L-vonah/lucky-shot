using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;

namespace LuckyShot.Domain.Services;

public interface ICompetitionInfoProvider
{
    Task<CompetitionInfoResult> FetchCompetitionInformation(CompetitionCategory category);
    Task<IEnumerable<CompetitionTeamsInfoResult>> FetchCompetitionTeamsInformation(CompetitionCategory category, int year);
    Task<IEnumerable<CompetitionMatchesInfoResult>> FetchCompetitionMatchesInformation(CompetitionCategory category, int year);
}