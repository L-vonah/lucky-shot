using LuckyShot.Domain.Entities;

namespace LuckyShot.Infrastructure.Extensions;

public static class MatchExtensions
{
    public static IQueryable<Match> WithExternalIds(this IQueryable<Match> query, ICollection<int> externalIds)
    {
        return query.Where(m => externalIds.Contains(m.ExternalId));
    }
    
    public static IQueryable<Match> FromSeason(this IQueryable<Match> query, int seasonId)
    {
        return query.Where(m => m.SeasonId == seasonId);
    }
}