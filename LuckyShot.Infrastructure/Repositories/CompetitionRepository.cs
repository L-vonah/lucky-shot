using LuckyShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class CompetitionRepository(LuckyShotContext context) : Repository<Competition, Guid>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Competitions.AnyAsync(t => t.Id == id);
    }
}