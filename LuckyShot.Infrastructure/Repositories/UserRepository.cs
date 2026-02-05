using LuckyShot.Domain.Entities;
using LuckyShot.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LuckyShot.Infrastructure.Repositories;

public class UserRepository(AuthContext context) : Repository<User, Guid>(context)
{
    private DbSet<User> Users => context.Set<User>();
    
    public async Task<User?> GetByEmailAsync(string email) =>
        await Users.SingleOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByEmailConfirmationTokenHashAsync(string tokenHash) =>
        await Users.SingleOrDefaultAsync(u => u.EmailConfirmationTokenHash == tokenHash);

    public async Task<bool> ExistsAsync(string email) =>
        await Users.AnyAsync(u => u.Email == email);

    public override async Task<bool> ExistsAsync(Guid id) =>
        await Users.AnyAsync(u => u.Id == id);
}