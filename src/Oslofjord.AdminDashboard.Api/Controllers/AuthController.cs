using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oslofjord.AdminDashboard.Api.Services;

namespace Oslofjord.AdminDashboard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint - FOR DEVELOPMENT ONLY
    /// In production, replace with proper user authentication (Azure AD B2C, etc.)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // TEMPORARY: Hardcoded credentials for development
        // TODO: Replace with proper user authentication
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required" });
        }

        // DEVELOPMENT ONLY: Accept these test users
        var (isValid, userId, roles) = ValidateCredentials(request.Email, request.Password);
        
        if (!isValid)
        {
            _logger.LogWarning("Failed login attempt for {Email}", request.Email);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = _tokenService.GenerateToken(userId, request.Email, roles);

        _logger.LogInformation("User {Email} logged in successfully", request.Email);

        return Ok(new LoginResponse
        {
            Token = token,
            Email = request.Email,
            ExpiresIn = 3600 // 1 hour in seconds
        });
    }

    /// <summary>
    /// Validate token endpoint - useful for frontend to check if token is still valid
    /// </summary>
    [HttpPost("validate")]
    [AllowAnonymous]
    public IActionResult ValidateToken([FromBody] ValidateTokenRequest request)
    {
        if (string.IsNullOrEmpty(request.Token))
        {
            return BadRequest(new { message = "Token is required" });
        }

        var principal = _tokenService.ValidateToken(request.Token);
        
        if (principal == null)
        {
            return Unauthorized(new { message = "Invalid or expired token" });
        }

        var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            valid = true,
            userId,
            email
        });
    }

    /// <summary>
    /// TEMPORARY: Hardcoded credential validation
    /// Replace with proper database lookup or Azure AD B2C
    /// </summary>
    private (bool isValid, string userId, string[] roles) ValidateCredentials(string email, string password)
    {
        // DEVELOPMENT CREDENTIALS ONLY - REMOVE IN PRODUCTION
        var devUsers = new Dictionary<string, (string password, string userId, string[] roles)>
        {
            ["admin@oslofjord.no"] = ("Admin123!", "user-001", new[] { "Admin", "User" }),
            ["user@oslofjord.no"] = ("User123!", "user-002", new[] { "User" }),
            ["api@oslofjord.no"] = ("Api123!", "user-003", new[] { "API", "Service" })
        };

        if (devUsers.TryGetValue(email.ToLower(), out var user))
        {
            if (user.password == password)
            {
                return (true, user.userId, user.roles);
            }
        }

        return (false, string.Empty, Array.Empty<string>());
    }
}

public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
}

public record ValidateTokenRequest
{
    public string Token { get; init; } = string.Empty;
}
