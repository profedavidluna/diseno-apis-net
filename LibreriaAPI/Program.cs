using LibreriaAPI.Data;
using LibreriaAPI.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers with Newtonsoft JSON (required for JSON Patch support)
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Response Caching
builder.Services.AddResponseCaching();

// EF Core - Base de datos en memoria
builder.Services.AddDbContext<LibreriaContext>(options =>
    options.UseInMemoryDatabase("LibreriaDB"));

// ──────────────────────────────────────────────────────────────
// Autenticación: JWT Bearer (default) + API Key (esquema custom)
// ──────────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    })
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationOptions.SchemeName, options =>
        {
            options.HeaderName = builder.Configuration["ApiKey:HeaderName"] ?? "X-API-Key";
            options.Value = builder.Configuration["ApiKey:Value"] ?? string.Empty;
        });

builder.Services.AddAuthorization();

// ──────────────────────────────────────────────────────────────
// Manejo global de errores con ProblemDetails (RFC 7807)
// ──────────────────────────────────────────────────────────────
builder.Services.AddProblemDetails();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Librería API",
        Version = "v1",
        Description = "API REST para gestionar libros, categorías y autores de una librería. " +
                      "Los endpoints GET requieren JWT Bearer. Los endpoints POST requieren API Key (X-API-Key).",
        Contact = new OpenApiContact
        {
            Name = "Librería",
            Email = "contacto@libreria.com"
        }
    });

    // Include XML comments for Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);

    // Seguridad: JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT. Ejemplo: Bearer {token}"
    });

    // Seguridad: API Key
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = "API Key para endpoints POST. Ingresa el valor de la clave."
    });

    // Aplicar ambos esquemas globalmente (Swagger los mostrará como opcionales)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed the in-memory database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibreriaContext>();
    context.Database.EnsureCreated();
}

// ──────────────────────────────────────────────────────────────
// Pipeline de middleware
// ──────────────────────────────────────────────────────────────

// Manejo global de excepciones (ProblemDetails, sin stack trace en respuesta)
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}
else
{
    // HSTS solo en producción
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
