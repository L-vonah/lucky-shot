using LuckyShot.Domain.Entities;
using LuckyShot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class SeasonRepository(CompetitionDataContext context) : Repository<Season, int>(context)
{
    private readonly CompetitionDataContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Seasons.AnyAsync(t => t.Id == id);
    }

    public async Task<Season?> GetByExternalIdAsync(int externalId)
    {
        return await _context.Seasons.FirstOrDefaultAsync(t => t.ExternalId == externalId);
    }
}