namespace LuckShot.Domain.Entities;

public class Season(
    int competitionId,
    string name,
    DateTime startDate,
    DateTime endDate,
    string externalId,
    int currentRound = 1) : DatabaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public DateTime StartDate { get; set; } = startDate;
    public DateTime EndDate { get; set; } = endDate;
    public string ExternalId { get; set; } = externalId;
    public int CurrentRound { get; set; } = currentRound;
    public int CompetitionId { get; set; } = competitionId;
    public Competition? Competition { get; set; }
    public ICollection<Match> Matches { get; set; } = [];
}