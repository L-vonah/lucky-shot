using LuckyShot.Domain.Entities;

namespace LuckyShot.Infrastructure.Extensions;

public static class TeamExtensions
{
    public static IQueryable<Team> WithExternalIds(this IQueryable<Team> query, ICollection<int> externalIds)
    {
        return query.Where(t => externalIds.Contains(t.ExternalId));
    }
}