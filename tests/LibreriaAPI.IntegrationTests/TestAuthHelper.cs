using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibreriaAPI.IntegrationTests;

public static class TestAuthHelper
{
    public const string TestJwtKey = "TEST-SECRET-KEY-FOR-INTEGRATION-TESTS-MIN32CHARS!";
    private const string TestIssuer = "LibreriaAPI";
    private const string TestAudience = "LibreriaAPI";

    public const string TestApiKey = "test-apikey-123";

    public static string GenerateJwtToken(string usuario = "testuser")
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, usuario)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static void AddBearerToken(HttpClient client, string? token = null)
    {
        var jwt = token ?? GenerateJwtToken();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
    }

    public static void AddApiKey(HttpClient client, string? apiKey = null)
    {
        client.DefaultRequestHeaders.Remove("X-API-Key");
        client.DefaultRequestHeaders.Add("X-API-Key", apiKey ?? TestApiKey);
    }
}
