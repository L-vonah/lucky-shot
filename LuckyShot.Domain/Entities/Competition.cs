namespace LuckyShot.Domain.Entities;

public class Competition(string name, string code, string logo, int externalId) : DatabaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public string Code { get; set; } = code;
    public string Logo { get; set; } = logo;
    public int ExternalId { get; set; } = externalId;
    public int? CurrentSeasonId { get; set; }
    public Season? CurrentSeason { get; set; }
}