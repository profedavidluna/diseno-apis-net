using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibreriaAPI.Controllers;

/// <summary>Autenticación: emisión de tokens JWT para pruebas</summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>Emite un JWT firmado con credenciales de demostración (configuradas en appsettings)</summary>
    /// <remarks>
    /// Credenciales de prueba (configuradas en appsettings.json bajo "DemoCredentials"):
    /// usuario = "admin", contraseña = "password123"
    ///
    /// Ejemplo de request:
    ///
    ///     POST /api/auth/token
    ///     {
    ///       "usuario": "admin",
    ///       "contrasena": "password123"
    ///     }
    /// </remarks>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<TokenResponse> GetToken(LoginRequest request)
    {
        var jwtKey = _config["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey))
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "JWT signing key is not configured." });

        // Credenciales leídas desde configuración (appsettings / secrets)
        var expectedUser = _config["DemoCredentials:Usuario"] ?? "admin";
        var expectedPass = _config["DemoCredentials:Contrasena"] ?? "password123";

        var userBytes = System.Text.Encoding.UTF8.GetBytes(request.Usuario ?? string.Empty);
        var expectedUserBytes = System.Text.Encoding.UTF8.GetBytes(expectedUser);
        var passBytes = System.Text.Encoding.UTF8.GetBytes(request.Contrasena ?? string.Empty);
        var expectedPassBytes = System.Text.Encoding.UTF8.GetBytes(expectedPass);

        bool userMatch = System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(userBytes, expectedUserBytes);
        bool passMatch = System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(passBytes, expectedPassBytes);

        if (!userMatch || !passMatch)
            return Unauthorized(new { message = "Credenciales inválidas." });

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Usuario!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, request.Usuario!)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new TokenResponse(tokenString, 3600));
    }
}

/// <summary>Request de login con credenciales dummy</summary>
public record LoginRequest(string Usuario, string Contrasena);

/// <summary>Respuesta con JWT emitido</summary>
public record TokenResponse(string Token, int ExpiresIn);
