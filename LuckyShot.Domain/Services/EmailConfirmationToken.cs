using System.Security.Cryptography;
using System.Text;

namespace LuckyShot.Domain.Services;

public static class EmailConfirmationToken
{
    /// <summary>
    /// Generates a random token for email confirmation links.
    /// </summary>
    /// <returns>Base64-encoded token string.</returns>
    public static string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Hashes an email confirmation token for secure storage.
    /// </summary>
    /// <param name="token">Raw token string to hash.</param>
    /// <returns>SHA256 hex string of the token.</returns>
    public static string Hash(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}