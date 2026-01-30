using LuckyShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class TeamRepository(LuckyShotContext context) : Repository<Team, int>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Teams.AnyAsync(t => t.Id == id);
    }
    
    public async Task<List<Team>> GetByExternalIdsAsync(IEnumerable<int> externalIds)
    {
        var externalIdList = externalIds.Distinct().ToArray();
        if (externalIdList.Length == 0) return [];

        return await _context.Teams
            .Where(t => externalIdList.Contains(t.ExternalId))
            .ToListAsync();
    }

    public IQueryable<Team> GetTeamsQuery() => _context.Teams;
}