namespace LuckyShot.Domain.Entities;

public class Season(
    DateTime? startDate,
    DateTime? endDate,
    int externalId,
    int? currentRound = 1) : DatabaseEntity
{
    public int Id { get; set; }
    public DateTime? StartDate { get; set; } = startDate;
    public DateTime? EndDate { get; set; } = endDate;
    public int ExternalId { get; set; } = externalId;
    public int? CurrentRound { get; set; } = currentRound;
    public Guid? CompetitionId { get; set; }
    public Competition? Competition { get; set; }
    public ICollection<Match> Matches { get; set; } = [];
}