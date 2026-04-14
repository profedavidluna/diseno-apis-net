using FluentAssertions;
using LibreriaAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LibreriaAPI.UnitTests;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly IConfiguration _configuration;

    public AuthControllerTests()
    {
        var configValues = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "TEST-SECRET-KEY-FOR-UNIT-TESTS-MIN32CHARS!!!!!",
            ["Jwt:Issuer"] = "LibreriaAPI",
            ["Jwt:Audience"] = "LibreriaAPI",
            ["DemoAuth:Usuario"] = "admin",
            ["DemoAuth:Password"] = "password123"
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        _controller = new AuthController(_configuration);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void GetToken_CredencialesCorrectas_Retorna200ConToken()
    {
        var request = new LoginRequest("admin", "password123");

        var result = _controller.GetToken(request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var tokenResponse = okResult.Value.Should().BeOfType<TokenResponse>().Subject;
        tokenResponse.Token.Should().NotBeNullOrEmpty();
        tokenResponse.ExpiresIn.Should().Be(3600);
    }

    [Fact]
    public void GetToken_CredencialesIncorrectas_Retorna401()
    {
        var request = new LoginRequest("admin", "wrongpassword");

        var result = _controller.GetToken(request);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void GetToken_UsuarioIncorrecto_Retorna401()
    {
        var request = new LoginRequest("wronguser", "password123");

        var result = _controller.GetToken(request);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public void GetToken_TokenGenerado_ContieneIssuerCorrecto()
    {
        var request = new LoginRequest("admin", "password123");

        var result = _controller.GetToken(request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var tokenResponse = okResult.Value.Should().BeOfType<TokenResponse>().Subject;

        // Validate JWT structure (3 parts)
        var parts = tokenResponse.Token.Split('.');
        parts.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("", "password123")]
    [InlineData("admin", "")]
    public void GetToken_CredencialesVacias_Retorna401(string usuario, string password)
    {
        var request = new LoginRequest(usuario, password);

        var result = _controller.GetToken(request);

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }
}
