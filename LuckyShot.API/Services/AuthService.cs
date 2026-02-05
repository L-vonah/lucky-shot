using LuckyShot.API.DTOs.Auth;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Services;
using LuckyShot.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace LuckyShot.API.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(string email, string password, string name);
    Task<AuthResponse> LoginAsync(string email, string password);
    Task<AuthResponse> RefreshTokenAsync(Guid userId);
    Task<UserResponse> GetCurrentUserAsync(Guid userId);
    Task ConfirmEmailAsync(string token);
}

public class AuthService(
    UserRepository userRepository,
    IAccessTokenIssuer tokenIssuer,
    IEmailSender emailSender,
    IPasswordHasher<User> passwordHasher,
    IConfiguration configuration
) : IAuthService
{
    private readonly int _accessTokenExpirationMinutes = GetAccessTokenExpirationMinutes(configuration);
    private readonly string _confirmBaseUrl = GetRequiredConfig(configuration, "Email:ConfirmBaseUrl");

    public async Task<AuthResponse> RegisterAsync(string email, string password, string name)
    {
        var normalizedEmail = NormalizeEmail(email);
        if (await userRepository.ExistsAsync(normalizedEmail))
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        var user = new User(normalizedEmail, string.Empty, name);
        user.PasswordHash = passwordHasher.HashPassword(user, password);
        var token = EmailConfirmationToken.Generate();
        var tokenHash = EmailConfirmationToken.Hash(token);
        user.SetEmailConfirmation(tokenHash, DateTimeOffset.UtcNow.AddHours(24));

        await userRepository.AddAsync(user);

        var confirmLink = $"{_confirmBaseUrl}?token={Uri.EscapeDataString(token)}";
        const string subject = "Lucky Shot - Confirm your email";
        var body = $"Welcome to Lucky Shot!\n\nClick the link below to confirm your email:\n{confirmLink}";
        await emailSender.SendAsync(user.Email, subject, body);

        var accessToken = tokenIssuer.IssueToken(user);
        return BuildAuthResponse(user, accessToken);
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidOperationException("Invalid credentials.");
        }

        user.RegisterLogin(DateTimeOffset.UtcNow);
        await userRepository.UpdateAsync(user);

        var token = tokenIssuer.IssueToken(user);
        return BuildAuthResponse(user, token);
    }

    public async Task<AuthResponse> RefreshTokenAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        var token = tokenIssuer.IssueToken(user);
        return BuildAuthResponse(user, token);
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        return MapUserResponse(user);
    }

    public async Task ConfirmEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token is required.");
        }

        var tokenHash = EmailConfirmationToken.Hash(token);
        var user = await userRepository.GetByEmailConfirmationTokenHashAsync(tokenHash);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid or expired token.");
        }

        if (user.IsEmailConfirmed) return;
        if (!user.EmailConfirmationExpiresAt.HasValue ||
            user.EmailConfirmationExpiresAt.Value <= DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Invalid or expired token.");
        }

        user.ConfirmEmail();
        await userRepository.UpdateAsync(user);
    }

    private AuthResponse BuildAuthResponse(User user, string token)
    {
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_accessTokenExpirationMinutes);
        return new AuthResponse(token, expiresAt, MapUserResponse(user));
    }

    private static UserResponse MapUserResponse(User user) =>
        new(
            user.Id,
            user.Email,
            user.Name,
            user.AvatarUrl,
            user.Role.ToString(),
            user.IsEmailConfirmed,
            user.LoginCount
        );

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static int GetAccessTokenExpirationMinutes(IConfiguration configuration)
    {
        var value = configuration["Jwt:AccessTokenExpirationMinutes"];
        if (int.TryParse(value, out var minutes)) return minutes;
        throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes is not configured.");
    }

    private static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{key} must be configured.");
        }

        return value;
    }
}