using System.Security.Claims;
using LuckyShot.Domain.Entities;

namespace LuckyShot.Domain.Services;

public interface IAccessTokenIssuer
{
    /// <summary>
    /// Issues a signed JWT access token for the specified user.
    /// </summary>
    /// <param name="user">User whose identity and role claims will be embedded in the token.</param>
    /// <returns>Signed JWT access token string.</returns>
    string IssueToken(User user);

    /// <summary>
    /// Validates a JWT access token and returns its claims principal when valid.
    /// </summary>
    /// <param name="token">JWT access token string to validate.</param>
    /// <returns>Claims principal when the token is valid; otherwise <c>null</c>.</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extracts the user id (sub) from a valid JWT access token.
    /// </summary>
    /// <param name="token">JWT access token string to validate and read.</param>
    /// <returns>User id when the token is valid and contains a subject claim; otherwise <c>null</c>.</returns>
    Guid? GetUserId(string token);
}