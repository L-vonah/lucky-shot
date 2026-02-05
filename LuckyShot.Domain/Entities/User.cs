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
    public int LoginCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAt { get; set; }
    public string? EmailConfirmationTokenHash { get; private set; }
    public DateTimeOffset? EmailConfirmationExpiresAt { get; private set; }

    public void RegisterLogin(DateTimeOffset loginAt)
    {
        LoginCount += 1;
        LastLoginAt = loginAt;
    }

    public void SetEmailConfirmation(string tokenHash, DateTimeOffset expiresAt)
    {
        EmailConfirmationTokenHash = tokenHash;
        EmailConfirmationExpiresAt = expiresAt;
        IsEmailConfirmed = false;
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
        EmailConfirmationTokenHash = null;
        EmailConfirmationExpiresAt = null;
    }
}