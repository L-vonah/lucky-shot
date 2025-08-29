using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.Repositories;

namespace LuckyShot.API.Services;

public interface ICompetitionSyncService
{
    Task SyncCompetitionsByCategoryAsync(CompetitionCategory category);
    Task SyncCompetitionTeamsAsync(CompetitionCategory category, int year);
    Task SyncMatchesFromCompetitionAsync(CompetitionCategory category, int year);
}

public class CompetitionSyncService(
    CompetitionRepository competitionRepository,
    SeasonRepository seasonRepository,
    ICompetitionInfoProvider competitionInfoProvider
) : ICompetitionSyncService
{
    public async Task SyncCompetitionsByCategoryAsync(CompetitionCategory category)
    {
        var competitionInfo = await competitionInfoProvider.FetchCompetitionInformation(category);
        var season = CreateSeasonFromInfo(competitionInfo);
        
        var existingCompetition = await competitionRepository.GetByExternalIdAsync(competitionInfo.ExternalId);
        if (existingCompetition == null)
        {
            var competition = new Competition(
                competitionInfo.Name,
                competitionInfo.Code,
                competitionInfo.Logo,
                competitionInfo.ExternalId
            ) { CurrentSeason = season };

            await competitionRepository.AddAsync(competition);
            existingCompetition = competition;
        }
        
        await UpdateSeasonIfNeeded(existingCompetition, competitionInfo);
        if (season?.StartDate != null)
        {
            var year = season.StartDate.Value.Year;
            await SyncCompetitionTeamsAsync(category, year);
        }
    }

    public Task SyncCompetitionTeamsAsync(CompetitionCategory category, int year)
    {
        throw new NotImplementedException();
    }

    public Task SyncMatchesFromCompetitionAsync(CompetitionCategory category, int year)
    {
        throw new NotImplementedException();
    }
    
    private static Season? CreateSeasonFromInfo(CompetitionInfoResult info)
    {
        if (info.CurrentSeasonId == null) return null;

        return new Season(
            info.CurrentSeasonStartDate,
            info.CurrentSeasonEndDate,
            info.CurrentSeasonId.Value,
            info.CurrentSeasonRound
        );
    }
    
    private async Task UpdateSeasonIfNeeded(Competition existingCompetition, CompetitionInfoResult info)
    {
        var season = existingCompetition.CurrentSeason;

        if (season == null || season.ExternalId != info.CurrentSeasonId)
        {
            existingCompetition.CurrentSeason = CreateSeasonFromInfo(info);
            existingCompetition.CurrentSeasonId = null;
            await competitionRepository.UpdateAsync(existingCompetition);
            return;
        }

        var updated = false;

        if (season.StartDate != info.CurrentSeasonStartDate)
        {
            season.StartDate = info.CurrentSeasonStartDate;
            updated = true;
        }
        if (season.EndDate != info.CurrentSeasonEndDate)
        {
            season.EndDate = info.CurrentSeasonEndDate;
            updated = true;
        }
        if (season.CurrentRound != info.CurrentSeasonRound)
        {
            season.CurrentRound = info.CurrentSeasonRound;
            updated = true;
        }

        if (updated)
            await seasonRepository.UpdateAsync(season);
    }
}