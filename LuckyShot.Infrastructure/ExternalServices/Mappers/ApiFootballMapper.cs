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
        var teams = summaryResponse.Teams;
        
        return new CompetitionSummaryResult(
            Competition: competition.ToDomainCompetitionInfoResult(),
            Season: currentSeason.ToDomainCompetitionSeasonInfoResult(),
            Teams: teams.Select(t => t.ToDomainCompetitionTeamsInfoResult()).ToArray()
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
            ExternalId: response.Id,
            Name: response.Name,
            ShortName: response.ShortName,
            Logo: response.Crest
        );
    }
    
    public static CompetitionMatchesInfoResult ToDomainCompetitionMatchesInfoResult(this MatchResponse response)
    {
        var homeTeam = response.HomeTeam;
        var awayTeam = response.AwayTeam;
        var score = response.Score;
        return new CompetitionMatchesInfoResult(
            ExternalId: response.Id,
            MatchDate: response.UtcDate,
            LastUpdatedDate: response.LastUpdated,
            Matchday: response.Matchday,
            Status: response.Status.ToDomainMatchStatus(),
            HomeTeamId: homeTeam.Id,
            AwayTeamId: awayTeam.Id,
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
            nameof(category), $"Not expected competition category value: {category}"),
    };
    
    private static MatchStatus ToDomainMatchStatus(this string status) => status.ToLower() switch
    {
        "scheduled" => MatchStatus.Scheduled,
        "live" => MatchStatus.Live,
        "in_play" => MatchStatus.InPlay,
        "paused" => MatchStatus.Paused,
        "finished" => MatchStatus.Finished,
        "postponed" => MatchStatus.Postponed,
        "suspended" => MatchStatus.Suspended,
        "cancelled" => MatchStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), $"Not expected match status value: {status}")
    };
}