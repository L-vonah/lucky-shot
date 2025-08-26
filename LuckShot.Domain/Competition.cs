namespace LuckShot.Domain;

public class Competition
{
    public Guid Uuid { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Logo { get; set; }
    public int ExternalId { get; set; }
    public int? CurrentSeasonId { get; set; }
    public Season? CurrentSeason { get; set; }

    public Competition(string name, string code, string logo, int externalId)
    {
        Uuid = Guid.NewGuid();
        Name = name;
        Code = code;
        Logo = logo;
        ExternalId = externalId;
    }
}