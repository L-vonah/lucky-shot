using LuckyShot.Domain.Entities;
using LuckyShot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class MatchRepository(CompetitionDataContext context) : Repository<Match, int>(context)
{
    private readonly CompetitionDataContext _context = context;

    public override async Task<bool> ExistsAsync(int id)
    {
        return await _context.Matches.AnyAsync(t => t.Id == id);
    }
    
    public IQueryable<Match> GetMatchesQuery() => _context.Matches;
}