using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Hateoas;
using LibreriaAPI.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// IMemoryCache – requerido por IdempotencyService
builder.Services.AddMemoryCache();

// HATEOAS – servicios que generan los links por tipo de recurso
builder.Services.AddScoped<IHateoasService<AutorDto>, AutorHateoasService>();
builder.Services.AddScoped<IHateoasService<CategoriaDto>, CategoriaHateoasService>();
builder.Services.AddScoped<IHateoasService<LibroDto>, LibroHateoasService>();

// Idempotencia – servicio singleton para cachear respuestas por clave
builder.Services.AddSingleton<IIdempotencyService, IdempotencyService>();
builder.Services.AddScoped<IdempotencyFilter>();

// EF Core - Base de datos en memoria
builder.Services.AddDbContext<LibreriaContext>(options =>
    options.UseInMemoryDatabase("LibreriaDB"));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Librería API",
        Version = "v1",
        Description = "API REST para gestionar libros, categorías y autores de una librería. Implementa HATEOAS: cada respuesta incluye una lista de links que describen las acciones disponibles sobre el recurso.",
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

    // Documenta el encabezado Idempotency-Key en todos los endpoints POST
    c.OperationFilter<IdempotencyOperationFilter>();
});

var app = builder.Build();

// Seed the in-memory database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibreriaContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
