using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LuckyShot.API.DTOs.Auth;
using LuckyShot.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuckyShot.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await authService.RegisterAsync(request.Email, request.Password, request.Name);
            return Created(string.Empty, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await authService.LoginAsync(request.Email, request.Password);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var userId = GetUserIdFromClaims(User);
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var response = await authService.RefreshTokenAsync(userId.Value);
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return Unauthorized();
        }
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var userId = GetUserIdFromClaims(User);
        if (!userId.HasValue) return Unauthorized();

        try
        {
            var response = await authService.GetCurrentUserAsync(userId.Value);
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    private static Guid? GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var subject = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                      ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(subject, out var userId)) return userId;
        return null;
    }
}

public record RegisterRequest(string Email, string Password, string Name);

public record LoginRequest(string Email, string Password);