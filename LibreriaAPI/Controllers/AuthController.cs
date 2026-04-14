using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibreriaAPI.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Genera un JWT para autenticación</summary>
    /// <remarks>
    /// Credenciales configuradas en DemoAuth:Usuario y DemoAuth:Password.
    /// Valor por defecto: usuario=admin, password=password123
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetToken([FromBody] LoginRequest request)
    {
        if (!IsValidUser(request.Usuario, request.Password))
            return Unauthorized(new { message = "Credenciales inválidas." });

        var token = GenerateJwtToken(request.Usuario);
        return Ok(new TokenResponse(token, 3600));
    }

    private bool IsValidUser(string usuario, string password)
    {
        var expectedUser = _configuration["DemoAuth:Usuario"] ?? "admin";
        var expectedPassword = _configuration["DemoAuth:Password"] ?? "password123";

        // Use constant-time comparison to prevent timing attacks
        var userMatch = string.Equals(usuario, expectedUser, StringComparison.Ordinal);
        var passMatch = string.Equals(password, expectedPassword, StringComparison.Ordinal);
        return userMatch && passMatch;
    }

    private string GenerateJwtToken(string usuario)
    {
        var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "LibreriaAPI";
        var audience = _configuration["Jwt:Audience"] ?? "LibreriaAPI";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, usuario)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record LoginRequest(string Usuario, string Password);
public record TokenResponse(string Token, int ExpiresIn);
