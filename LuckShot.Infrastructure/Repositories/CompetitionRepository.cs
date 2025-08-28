using LuckShot.Domain;
using LuckShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckShot.Infrastructure.Repositories;

public class CompetitionRepository(LuckyShotContext context) : Repository<Competition, Guid>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Competitions.AnyAsync(t => t.Uuid == id);
    }
}