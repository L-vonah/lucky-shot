namespace LuckyShot.API.DTOs.Auth;

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