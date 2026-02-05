namespace LuckyShot.API.DTOs.Auth;

public record RegisterRequest(string Email, string Password, string Name);

public record LoginRequest(string Email, string Password);

public record AuthResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    UserResponse User
);

public record UserResponse(
    Guid Id,
    string Email,
    string Name,
    string? AvatarUrl,
    string Role,
    bool IsEmailConfirmed,
    int LoginCount
);