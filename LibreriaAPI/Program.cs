using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Commands;
using LibreriaAPI.CQRS.Autores.Handlers;
using LibreriaAPI.CQRS.Autores.Queries;
using LibreriaAPI.CQRS.Categorias.Commands;
using LibreriaAPI.CQRS.Categorias.Handlers;
using LibreriaAPI.CQRS.Categorias.Queries;
using LibreriaAPI.CQRS.Dispatcher;
using LibreriaAPI.CQRS.Libros.Commands;
using LibreriaAPI.CQRS.Libros.Handlers;
using LibreriaAPI.CQRS.Libros.Queries;
using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// EF Core - Base de datos en memoria
builder.Services.AddDbContext<LibreriaContext>(options =>
    options.UseInMemoryDatabase("LibreriaDB"));

// Unit of Work (repositories are created internally by UnitOfWork using the shared context)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// CQRS – Dispatcher central
builder.Services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();

// CQRS – Handlers de Autores
builder.Services.AddScoped<IQueryHandler<GetAutoresQuery, IEnumerable<AutorDto>>, GetAutoresHandler>();
builder.Services.AddScoped<IQueryHandler<GetAutorByIdQuery, AutorDto?>, GetAutorByIdHandler>();
builder.Services.AddScoped<ICommandHandler<CreateAutorCommand, AutorDto>, CreateAutorHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateAutorCommand, AutorDto?>, UpdateAutorHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteAutorCommand, bool>, DeleteAutorHandler>();

// CQRS – Handlers de Categorias
builder.Services.AddScoped<IQueryHandler<GetCategoriasQuery, IEnumerable<CategoriaDto>>, GetCategoriasHandler>();
builder.Services.AddScoped<IQueryHandler<GetCategoriaByIdQuery, CategoriaDto?>, GetCategoriaByIdHandler>();
builder.Services.AddScoped<ICommandHandler<CreateCategoriaCommand, CategoriaDto>, CreateCategoriaHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateCategoriaCommand, CategoriaDto?>, UpdateCategoriaHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteCategoriaCommand, bool>, DeleteCategoriaHandler>();

// CQRS – Handlers de Libros
builder.Services.AddScoped<IQueryHandler<GetLibrosQuery, IEnumerable<LibroDto>>, GetLibrosHandler>();
builder.Services.AddScoped<IQueryHandler<GetLibroByIdQuery, LibroDto?>, GetLibroByIdHandler>();
builder.Services.AddScoped<IQueryHandler<GetLibrosByCategoriaQuery, IEnumerable<LibroDto>>, GetLibrosByCategoriaHandler>();
builder.Services.AddScoped<ICommandHandler<CreateLibroCommand, CreateLibroResult>, CreateLibroHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateLibroCommand, UpdateLibroResult>, UpdateLibroHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteLibroCommand, bool>, DeleteLibroHandler>();

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
