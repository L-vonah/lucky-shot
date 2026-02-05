using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LuckyShot.Domain.Entities;
using LuckyShot.Domain.Services;
using Microsoft.IdentityModel.Tokens;

namespace LuckyShot.API.Services;

public class JwtService : IAccessTokenIssuer
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly TokenValidationParameters _validationParameters;
    private readonly RsaSecurityKey _privateKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;

    public JwtService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

        var expirationValue = configuration["Jwt:AccessTokenExpirationMinutes"];
        if (!int.TryParse(expirationValue, out _accessTokenExpirationMinutes))
        {
            throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes is not configured.");
        }

        var privateKeyPem = configuration["Jwt:PrivateKeyPem"];
        var privateKeyPassphrase = configuration["Jwt:PrivateKeyPassphrase"];
        _privateKey = new RsaSecurityKey(LoadRsaKey(privateKeyPem, privateKeyPassphrase));

        var publicKeyPem = configuration["Jwt:PublicKeyPem"];
        var publicKey = new RsaSecurityKey(LoadRsaKey(publicKeyPem, null));

        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = publicKey,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }

    public string IssueToken(User user)
    {
        var now = DateTimeOffset.UtcNow;
        var expires = now.AddMinutes(_accessTokenExpirationMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credentials = new SigningCredentials(_privateKey, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);

        return _tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            return _tokenHandler.ValidateToken(token, _validationParameters, out _);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }

    public Guid? GetUserId(string token)
    {
        var principal = ValidateToken(token);
        var subject = principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(subject, out var userId)) return userId;
        return null;
    }

    private static RSA LoadRsaKey(string? pem, string? passphrase)
    {
        if (string.IsNullOrWhiteSpace(pem))
        {
            throw new InvalidOperationException("JWT key is not configured.");
        }

        var rsa = RSA.Create();
        if (!string.IsNullOrWhiteSpace(passphrase))
        {
            try
            {
                rsa.ImportFromEncryptedPem(pem, passphrase);
                return rsa;
            }
            catch (CryptographicException)
            {
                // Fallback for unencrypted key
            }
        }

        rsa.ImportFromPem(pem);
        return rsa;
    }
}