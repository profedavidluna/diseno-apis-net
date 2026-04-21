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

    // Include XML comments for Swagger
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
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

    // Seed Libros read models (denormalized)
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
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
