#if DEBUG // Only include this controller in Debug builds

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace PingyThingy.Api.Controllers;

[ApiController]
[Route("dev")] // Base route for development endpoints
[AllowAnonymous] // Allow access without authentication
[ApiExplorerSettings(GroupName = "Development")] // Group in Swagger
public class DevelopmentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DevelopmentController> _logger;

    public DevelopmentController(
        IConfiguration configuration,
        ILogger<DevelopmentController> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("generate-token")]
    public IActionResult GenerateToken()
    {
        _logger.LogInformation("Generating development token.");

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var keyString = _configuration["Jwt:Key"];

        if (
            string.IsNullOrEmpty(issuer)
            || string.IsNullOrEmpty(audience)
            || string.IsNullOrEmpty(keyString)
        )
        {
            _logger.LogError(
                "JWT Issuer, Audience, or Key is not configured correctly in appsettings."
            );
            return Problem(
                title: "JWT Configuration Error",
                detail: "Ensure Jwt:Issuer, Jwt:Audience, and Jwt:Key are set in configuration.",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }

        var key = Encoding.UTF8.GetBytes(keyString);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, "testuser@example.com"), // Example user ID
                    new Claim(JwtRegisteredClaimNames.Email, "testuser@example.com"),
                    // Use NameIdentifier for the user ID claim used by the rate limiter policy
                    new Claim(JwtRegisteredClaimNames.NameId, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(15), // Increased expiration slightly
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature
            ),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new { token = jwtToken });
    }
}

#endif // DEBUG
