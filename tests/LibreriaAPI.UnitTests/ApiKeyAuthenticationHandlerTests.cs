using FluentAssertions;
using LibreriaAPI.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Text.Encodings.Web;

namespace LibreriaAPI.UnitTests;

public class ApiKeyAuthenticationHandlerTests
{
    private static async Task<ApiKeyAuthenticationHandler> BuildHandler(
        HttpContext context,
        IConfiguration configuration)
    {
        var options = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
        options.Setup(o => o.Get(It.IsAny<string>())).Returns(new ApiKeyAuthenticationOptions());
        options.Setup(o => o.CurrentValue).Returns(new ApiKeyAuthenticationOptions());

        var loggerFactory = new LoggerFactory();
        var encoder = UrlEncoder.Default;

        var handler = new ApiKeyAuthenticationHandler(options.Object, loggerFactory, encoder, configuration);

        var scheme = new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler));
        await handler.InitializeAsync(scheme, context);

        return handler;
    }

    private static IConfiguration BuildConfig(string? apiKeyValue)
    {
        var values = new Dictionary<string, string?> { ["ApiKey:Value"] = apiKeyValue };
        return new ConfigurationBuilder().AddInMemoryCollection(values).Build();
    }

    [Fact]
    public async Task HandleAuthenticate_SinHeader_Retorna_Fail()
    {
        var context = new DefaultHttpContext();
        var config = BuildConfig("my-secret-key");
        var handler = await BuildHandler(context, config);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Contain("header not found");
    }

    [Fact]
    public async Task HandleAuthenticate_ApiKeyCorrecta_Retorna_Success()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = "my-secret-key";
        var config = BuildConfig("my-secret-key");
        var handler = await BuildHandler(context, config);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeTrue();
        result.Principal.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAuthenticate_ApiKeyIncorrecta_Retorna_Fail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = "wrong-key";
        var config = BuildConfig("my-secret-key");
        var handler = await BuildHandler(context, config);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeFalse();
        result.Failure!.Message.Should().Contain("Invalid API Key");
    }

    [Fact]
    public async Task HandleAuthenticate_ApiKeyNulaEnConfig_Retorna_Fail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-API-Key"] = "some-key";
        var config = BuildConfig(null);
        var handler = await BuildHandler(context, config);

        var result = await handler.AuthenticateAsync();

        result.Succeeded.Should().BeFalse();
    }
}
