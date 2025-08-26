using LuckShot.Domain;
using Microsoft.EntityFrameworkCore;

namespace LuckShot.Infrastructure.Repositories;

public class SeasonRepository(LuckyShotContext context) : Repository<Season, int>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Seasons.AnyAsync(t => t.Id == id);
    }
}