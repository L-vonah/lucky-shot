using LuckShot.Domain;
using LuckShot.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LuckShot.Infrastructure.Repositories;

public class TeamRepository(LuckyShotContext context) : Repository<Team, int>(context)
{
    private readonly LuckyShotContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Teams.AnyAsync(t => t.Id == id);
    }
}