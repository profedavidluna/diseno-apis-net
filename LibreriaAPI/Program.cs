using Asp.Versioning;
using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models.ReadModels;
using LibreriaAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// API Versioning – header (X-API-Version) and query param (?api-version)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("X-API-Version"),
        new QueryStringApiVersionReader("api-version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// EF Core - BD de escritura (Write DB)
builder.Services.AddDbContext<LibreriaContext>(options =>
    options.UseInMemoryDatabase("LibreriaWriteDB"));

// EF Core - BD de lectura (Read DB)
builder.Services.AddDbContext<LibreriaReadContext>(options =>
    options.UseInMemoryDatabase("LibreriaReadDB"));

// Write Repositories
builder.Services.AddScoped<IAutoresRepository, AutoresRepository>();
builder.Services.AddScoped<ICategoriasRepository, CategoriasRepository>();
builder.Services.AddScoped<ILibrosRepository, LibrosRepository>();

// Read Repositories
builder.Services.AddScoped<IAutoresReadRepository, AutoresReadRepository>();
builder.Services.AddScoped<ICategoriasReadRepository, CategoriasReadRepository>();
builder.Services.AddScoped<ILibrosReadRepository, LibrosReadRepository>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Librería API",
        Version = "v1",
        Description = "API REST para gestionar libros, categorías y autores de una librería.",
        Contact = new OpenApiContact
        {
            Name = "Librería",
            Email = "contacto@libreria.com"
        }
    });

    c.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Librería API",
        Version = "v2",
        Description = "API REST para gestionar libros, categorías y autores de una librería (versión 2).",
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

    // Only show endpoints that belong to each Swagger doc version
    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (apiDesc.GroupName is null)
            return docName == "v1";
        return apiDesc.GroupName == docName;
    });
});

var app = builder.Build();

// Seed Write DB and project initial data to Read DB
using (var scope = app.Services.CreateScope())
{
    var writeContext = scope.ServiceProvider.GetRequiredService<LibreriaContext>();
    writeContext.Database.EnsureCreated();

    var readContext = scope.ServiceProvider.GetRequiredService<LibreriaReadContext>();

    // Seed Categorias read models
    foreach (var c in writeContext.Categorias)
        readContext.Categorias.Add(new CategoriaReadModel { Id = c.Id, Nombre = c.Nombre, Descripcion = c.Descripcion });

    // Seed Autores read models
    foreach (var a in writeContext.Autores)
        readContext.Autores.Add(new AutorReadModel { Id = a.Id, Nombre = a.Nombre, Apellido = a.Apellido, Biografia = a.Biografia });

    // Seed Libros read models (denormalized) + índice libro-autor
    var libros = writeContext.Libros
        .Include(l => l.Categoria)
        .Include(l => l.LibroAutores)
            .ThenInclude(la => la.Autor)
        .ToList();

    foreach (var l in libros)
    {
        var autoresDto = l.LibroAutores
            .Select(la => new AutorDto(la.Autor!.Id, la.Autor.Nombre, la.Autor.Apellido, la.Autor.Biografia))
            .ToList();

        readContext.Libros.Add(new LibroReadModel
        {
            Id = l.Id,
            Titulo = l.Titulo,
            Descripcion = l.Descripcion,
            ISBN = l.ISBN,
            AnioPublicacion = l.AnioPublicacion,
            CategoriaId = l.CategoriaId,
            CategoriaNombre = l.Categoria?.Nombre,
            AutoresJson = JsonSerializer.Serialize(autoresDto)
        });

        foreach (var la in l.LibroAutores)
            readContext.LibroAutores.Add(new LibroAutorReadModel { LibroId = l.Id, AutorId = la.AutorId });
    }

    readContext.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Librería API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Librería API v2");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
