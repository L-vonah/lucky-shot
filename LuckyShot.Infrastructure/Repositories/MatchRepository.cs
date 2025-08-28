using LuckyShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class MatchRepository(LuckyShotContext context) : Repository<Match, int>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Matches.AnyAsync(t => t.Id == id);
    }
}