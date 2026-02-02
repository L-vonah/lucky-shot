using LuckyShot.Domain.Entities;
using LuckyShot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class CompetitionRepository(CompetitionDataContext context) : Repository<Competition, Guid>(context)
{
    private readonly CompetitionDataContext _context = context;

    public override async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Competitions.AnyAsync(t => t.Id == id);
    }
    
    public async Task<Competition?> GetByExternalIdAsync(int externalId)
    {
        return await _context.Competitions
            .Include(c => c.CurrentSeason)
            .FirstOrDefaultAsync(c => c.ExternalId == externalId);
    }
}