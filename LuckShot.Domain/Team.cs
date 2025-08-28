namespace LuckShot.Domain;

public class Team(string name, string logo, int externalId) : DatabaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = name;
    public string Logo { get; set; } = logo;
    public int ExternalId { get; set; } = externalId;
}