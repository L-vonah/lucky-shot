namespace LuckyShot.Domain.Entities;

public class User(string email, string passwordHash, string name) : DatabaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public string Name { get; set; } = name;
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsEmailConfirmed { get; set; }
    public int LoginCount { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }

    public void RegisterLogin(DateTimeOffset loginAt)
    {
        LoginCount += 1;
        LastLoginAt = loginAt;
    }
}