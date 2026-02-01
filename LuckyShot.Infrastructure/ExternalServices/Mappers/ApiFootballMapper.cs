using ApiFootball.Dtos;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Models;

namespace LuckyShot.Infrastructure.ExternalServices.Mappers;

public static class ApiFootballMapper
{
    public static CompetitionSummaryResult ToDomainCompetitionSummaryResult(this CompetitionSummaryResponse summaryResponse)
    {
        var competition = summaryResponse.Competition;
        var currentSeason = summaryResponse.Season;
        var teams = summaryResponse.Teams.Select(t 
            => t.ToDomainCompetitionTeamsInfoResult()).ToArray();
        
        return new CompetitionSummaryResult(
            Competition: competition.ToDomainCompetitionInfoResult(),
            Season: currentSeason.ToDomainCompetitionSeasonInfoResult(),
            Teams: teams
        );
    }
    
    private static CompetitionInfoResult ToDomainCompetitionInfoResult(this CompetitionResponse response)
    {
        return new CompetitionInfoResult(
            ExternalId: response.Id,
            Name: response.Name,
            Code: response.Code,
            Logo: response.Emblem
        );
    }
    
    private static CompetitionSeasonInfoResult ToDomainCompetitionSeasonInfoResult(this CompetitionSeasonResponse response)
    {
        return new CompetitionSeasonInfoResult(
            ExternalId: response.Id,
            StartDate: response.StartDate,
            EndDate: response.EndDate,
            CurrentRound: response.CurrentMatchday
        );
    }
    
    private static CompetitionTeamInfoResult ToDomainCompetitionTeamsInfoResult(this TeamResponse response)
    {
        return new CompetitionTeamInfoResult(
            ExternalId: response.Id!.Value,
            Name: response.Name,
            Logo: response.Crest
        );
    }
    
    public static CompetitionMatchesInfoResult ToDomainCompetitionMatchesInfoResult(this CompetitionMatchResponse response)
    {
        var seasonId = response.Matches.First().Season.Id;
        var matches = response.Matches
            .Where(m => (m.HomeTeam.Id != null || m.AwayTeam.Id != null) && m.Matchday.HasValue)
            .Select(m => m.ToDomainCompetitionMatchesInfoResult())
            .ToArray();
        return new CompetitionMatchesInfoResult(
            SeasonExternalId: seasonId,
            Matches: matches
        );
    }
    
    private static MatchesInfoResult ToDomainCompetitionMatchesInfoResult(this MatchResponse response)
    {
        var score = response.Score;
        return new MatchesInfoResult(
            ExternalId: response.Id,
            MatchDate: response.UtcDate,
            LastUpdatedDate: response.LastUpdated,
            Round: response.Matchday!.Value,
            Status: response.Status.ToDomainMatchStatus(),
            Result: score.Winner.ToDomainMatchResult(),
            HomeTeamExternalId: response.HomeTeam.Id,
            AwayTeamExternalId: response.AwayTeam.Id,
            HomeTeamFullTimeScore: score.FullTime.Home,
            AwayTeamFullTimeScore: score.FullTime.Away,
            HomeTeamHalfTimeScore: score.HalfTime.Home,
            AwayTeamHalfTimeScore: score.HalfTime.Away
        );
    }
    
    public static string ToApiFootballCompetitionAcronym(this CompetitionCategory category) => category switch
    {
        CompetitionCategory.WorldCup => "WC",
        CompetitionCategory.ChampionsLeague => "CL",
        CompetitionCategory.Bundesliga => "BL1",
        CompetitionCategory.Eredivisie => "DED",
        CompetitionCategory.Brasileirao => "BSA",
        CompetitionCategory.LaLiga => "PD",
        CompetitionCategory.Ligue1 => "FL1",
        CompetitionCategory.PrimeiraLiga => "PPL",
        CompetitionCategory.SerieA => "SA",
        CompetitionCategory.PremierLeague => "PL",
        _ => throw new ArgumentOutOfRangeException(
            nameof(category), $"Not expected competition category value: {category}")
    };
    
    private static MatchStatus ToDomainMatchStatus(this string status) => status.ToLower() switch
    {
        "scheduled" or "timed" => MatchStatus.Scheduled,
        "live" => MatchStatus.Live,
        "in_play" => MatchStatus.InPlay,
        "paused" => MatchStatus.Paused,
        "finished" => MatchStatus.Finished,
        "postponed" => MatchStatus.Postponed,
        "suspended" => MatchStatus.Suspended,
        "cancelled" => MatchStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected match status value: {status}")
    };
    
    private static MatchResult ToDomainMatchResult(this string? result) => result?.ToLower() switch
    {
        "home_team" => MatchResult.HomeWin,
        "away_team" => MatchResult.AwayWin,
        "draw" => MatchResult.Draw,
        "null" or null => MatchResult.NotPlayed,
        _ => throw new ArgumentOutOfRangeException(nameof(result), $"Not expected match result value: {result}")
    };
}