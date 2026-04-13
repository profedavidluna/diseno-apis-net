using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LibreriaAPI.Security;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string SchemeName = "ApiKey";
    public string HeaderName { get; set; } = "X-API-Key";
    public string Value { get; set; } = string.Empty;
}

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var apiKeyValues))
            return Task.FromResult(AuthenticateResult.Fail("API Key header not found."));

        var providedKey = apiKeyValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedKey))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));

        // Constant-time comparison to prevent timing attacks
        var expectedKeyBytes = System.Text.Encoding.UTF8.GetBytes(Options.Value);
        var providedKeyBytes = System.Text.Encoding.UTF8.GetBytes(providedKey);
        if (!System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(expectedKeyBytes, providedKeyBytes))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key."));

        var claims = new[] { new Claim(ClaimTypes.Name, "ApiKeyUser") };
        var identity = new ClaimsIdentity(claims, ApiKeyAuthenticationOptions.SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
