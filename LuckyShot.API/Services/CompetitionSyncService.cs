using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.Extensions;
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
    MatchRepository matchRepository,
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

     public async Task SyncCompetitionSeasonMatchesAsync(CompetitionCategory category, int year)
    {
        var (seasonExternalId, matchesInfoResults)
            = await competitionInfoProvider.FetchCompetitionMatchesInformation(category, year);

        var season = await seasonRepository.GetByExternalIdAsync(seasonExternalId);
        if (season is null) return;

        MatchesInfoResult[] matchesToSync;
        if (season.MatchesLastUpdated is null)
        {
            matchesToSync = matchesInfoResults;
        }
        else
        {
            var lastUpdatedDate = season.MatchesLastUpdated.Value;
            matchesToSync = matchesInfoResults
                .Where(m => m.LastUpdatedDate > lastUpdatedDate)
                .ToArray();
        }

        await SyncMatchesAsync(matchesToSync, season.Id);

        if (matchesInfoResults.Length > 0)
        {
            season.MatchesLastUpdated = DateTime.UtcNow;
            await seasonRepository.UpdateAsync(season);
        }
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
            null,
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
            .WithExternalIds(externalIds)
            .ToDictionaryAsync(t => t.ExternalId, t => t);

        var teamsToAdd = new List<Team>();
        var teamsToUpdate = new List<Team>();

        foreach (var teamInfo in teamsInfo)
        {
            if (existingTeams.TryGetValue(teamInfo.ExternalId, out var existingTeam))
            {
                var shouldUpdate = existingTeam.Name != teamInfo.Name || existingTeam.Logo != teamInfo.Logo;
                if (!shouldUpdate) continue;
                
                existingTeam.Name = teamInfo.Name;
                existingTeam.Logo = teamInfo.Logo;
                teamsToUpdate.Add(existingTeam);
                continue;
            }
            
            var newTeam = new Team(teamInfo.Name, teamInfo.Logo, teamInfo.ExternalId);
            teamsToAdd.Add(newTeam);
        }
        
        if (teamsToAdd.Count > 0) await teamRepository.AddRangeAsync(teamsToAdd);
        if (teamsToUpdate.Count > 0) await teamRepository.UpdateRangeAsync(teamsToUpdate);
    }
    
    private async Task SyncMatchesAsync(MatchesInfoResult[] matchesInfo, int seasonId)
    {
        if (matchesInfo.Length == 0) return;
        var teamMap = await BuildTeamIdMapAsync(matchesInfo);

        var matchExternalIds = matchesInfo.Select(m => m.ExternalId).ToHashSet();
        var matches = await matchRepository.GetMatchesQuery()
            .FromSeason(seasonId).WithExternalIds(matchExternalIds)
            .ToDictionaryAsync(m => m.ExternalId, m => m);
        
        var matchesToAdd = new List<Match>();
        var matchesToUpdate = new List<Match>();
        
        foreach (var matchInfo in matchesInfo)
        {
            if (matches.TryGetValue(matchInfo.ExternalId, out var existingMatch))
            {
                var shouldUpdate = existingMatch.Date != matchInfo.MatchDate ||
                                   existingMatch.Status != matchInfo.Status ||
                                   existingMatch.Result != matchInfo.Result ||
                                   existingMatch.HomeTeamScore != matchInfo.HomeTeamFullTimeScore ||
                                   existingMatch.AwayTeamScore != matchInfo.AwayTeamFullTimeScore;
                if (!shouldUpdate) continue;
                
                existingMatch.Date = matchInfo.MatchDate;
                existingMatch.Status = matchInfo.Status;
                existingMatch.Result = matchInfo.Result;
                existingMatch.HomeTeamScore = matchInfo.HomeTeamFullTimeScore;
                existingMatch.AwayTeamScore = matchInfo.AwayTeamFullTimeScore;
                
                matchesToUpdate.Add(existingMatch);
                continue;
            }
            
            var newMatch = new Match(
                seasonId,
                matchInfo.MatchDate,
                matchInfo.Status,
                matchInfo.Result,
                matchInfo.Round,
                teamMap[matchInfo.HomeTeamId],
                teamMap[matchInfo.AwayTeamId],
                matchInfo.ExternalId
            )
            {
                HomeTeamScore = matchInfo.HomeTeamFullTimeScore,
                AwayTeamScore = matchInfo.AwayTeamFullTimeScore
            };
            
            matchesToAdd.Add(newMatch);
        }
        
        if (matchesToAdd.Count > 0) await matchRepository.AddRangeAsync(matchesToAdd);
        if (matchesToUpdate.Count > 0) await matchRepository.UpdateRangeAsync(matchesToUpdate);
    }

    private async Task<Dictionary<int, int>> BuildTeamIdMapAsync(IEnumerable<MatchesInfoResult> matchesInfo)
    {
        var teamExternalIds = matchesInfo
            .SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId })
            .ToHashSet();
        var teams = await teamRepository.GetByExternalIdsAsync(teamExternalIds);
        var teamMap = teams.ToDictionary(t => t.ExternalId, t => t.Id);
        var missingTeamIds = teamExternalIds.Except(teamMap.Keys).ToArray();
        if (missingTeamIds.Length > 0)
        {
            throw new InvalidOperationException(
                $"Times ausentes para os ExternalIds: {string.Join(", ", missingTeamIds)}.");
        }

        return teamMap;
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