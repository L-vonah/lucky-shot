using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.API.Services;

public interface ICompetitionSyncService
{
    Task SyncCompetitionsSummaryAsync(CompetitionCategory category, int year);
    Task SyncCompetitionSeasonMatchesAsync(CompetitionCategory category, int year);
}

public class CompetitionSyncService(
    CompetitionRepository competitionRepository,
    SeasonRepository seasonRepository,
    TeamRepository teamRepository,
    ICompetitionInfoProvider competitionInfoProvider
) : ICompetitionSyncService
{
    public async Task SyncCompetitionsSummaryAsync(CompetitionCategory category, int year)
    {
        var (competitionInfo, seasonInfo, teamsInfo)
            = await competitionInfoProvider.FetchCompetitionSummary(category, year);

        var competition = await SyncCompetitionsAsync(competitionInfo);
        var season = await SyncSeasonAsync(seasonInfo, competition.Id);

        if (competition.CurrentSeasonId != season.Id)
        {
            competition.CurrentSeasonId = season.Id;
            await competitionRepository.UpdateAsync(competition);
        }
        
        await SyncTeamsAsync(teamsInfo);
    }

    public Task SyncCompetitionSeasonMatchesAsync(CompetitionCategory category, int year)
    {
        throw new NotImplementedException();
    }

    private async Task<Competition> SyncCompetitionsAsync(CompetitionInfoResult competitionInfo)
    {
        var existingCompetition = await competitionRepository.GetByExternalIdAsync(competitionInfo.ExternalId);
        if (existingCompetition != null) return existingCompetition;
        
        var competition = new Competition(
            competitionInfo.Name,
            competitionInfo.Code,
            competitionInfo.Logo,
            competitionInfo.ExternalId
        );

        await competitionRepository.AddAsync(competition);
        return competition;
    }
    
    private async Task<Season> SyncSeasonAsync(CompetitionSeasonInfoResult seasonInfo, Guid competitionId)
    {
        var existingSeason = await seasonRepository.GetByExternalIdAsync(seasonInfo.ExternalId);
        if (existingSeason != null)
        {
            return await UpdateSeasonIfNeeded(existingSeason, seasonInfo);
        }

        var season = new Season(
            seasonInfo.StartDate,
            seasonInfo.EndDate,
            seasonInfo.ExternalId,
            seasonInfo.CurrentRound
        ) { CompetitionId = competitionId };

        await seasonRepository.AddAsync(season);
        return season;
    }
    
    private async Task SyncTeamsAsync(CompetitionTeamInfoResult[] teamsInfo)
    {
        if (teamsInfo.Length == 0) return;

        var externalIds = teamsInfo.Select(t => t.ExternalId).ToHashSet();
        var existingTeams = await teamRepository.GetTeamsQuery()
            .Where(t => externalIds.Contains(t.ExternalId))
            .ToDictionaryAsync(t => t.ExternalId, t => t);
        
        List<Team> teamsToAdd = [];
        List<Team> teamsToUpdate = [];
        
        foreach (var teamInfo in teamsInfo)
        {
            if (existingTeams.TryGetValue(teamInfo.ExternalId, out var existingTeam))
            {
                var shouldUpdate = existingTeam.Name != teamInfo.Name || existingTeam.Logo != teamInfo.Logo;
                if (!shouldUpdate) continue;
                
                existingTeam.Name = teamInfo.Name;
                existingTeam.Logo = teamInfo.Logo;
                teamsToUpdate.Add(existingTeam);
            }
            else
            {
                var newTeam = new Team(teamInfo.Name, teamInfo.Logo, teamInfo.ExternalId);
                teamsToAdd.Add(newTeam);
            }
        }
        
        if (teamsToAdd.Count > 0) await teamRepository.AddRangeAsync(teamsToAdd);
        if (teamsToUpdate.Count > 0) await teamRepository.UpdateRangeAsync(teamsToUpdate);
    }

    private async Task<Season> UpdateSeasonIfNeeded(Season existingSeason, CompetitionSeasonInfoResult info)
    {
        var shouldUpdate = existingSeason.StartDate != info.StartDate ||
                           existingSeason.EndDate != info.EndDate ||
                           existingSeason.CurrentRound != info.CurrentRound;
        if (!shouldUpdate) return existingSeason;
        
        existingSeason.StartDate = info.StartDate;
        existingSeason.EndDate = info.EndDate;
        existingSeason.CurrentRound = info.CurrentRound;

        await seasonRepository.UpdateAsync(existingSeason);
        return existingSeason;
    }
}