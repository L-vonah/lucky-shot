namespace LuckyShot.Domain.Entities;

public class Season(
    DateTimeOffset? startDate,
    DateTimeOffset? endDate,
    int externalId,
    DateTimeOffset? matchesLastUpdated,
    int? currentRound) : DatabaseEntity
{
    public int Id { get; set; }
    public DateTimeOffset? StartDate { get; set; } = startDate;
    public DateTimeOffset? EndDate { get; set; } = endDate;
    public int ExternalId { get; set; } = externalId;
    public DateTimeOffset? MatchesLastUpdated { get; set; } = matchesLastUpdated;
    public int? CurrentRound { get; set; } = currentRound;
    public Guid? CompetitionId { get; set; }
    public Competition? Competition { get; set; }
    public ICollection<Match> Matches { get; set; } = [];
}