using LibreriaAPI.Data;
using LibreriaAPI.Events;
using LibreriaAPI.Models.ReadModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un libro es actualizado en la BD de escritura.
/// </summary>
public class LibroActualizadoEventHandler : INotificationHandler<LibroActualizadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public LibroActualizadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(LibroActualizadoEvent notification, CancellationToken cancellationToken)
    {
        var libro = notification.Libro;
        var readModel = await _readContext.Libros.FindAsync([libro.Id], cancellationToken);

        if (readModel is null) return;

        readModel.Titulo = libro.Titulo;
        readModel.Descripcion = libro.Descripcion;
        readModel.ISBN = libro.ISBN;
        readModel.AnioPublicacion = libro.AnioPublicacion;
        readModel.CategoriaId = libro.CategoriaId;
        readModel.CategoriaNombre = libro.CategoriaNombre;
        readModel.AutoresJson = JsonSerializer.Serialize(libro.Autores);

        // Actualizar la tabla de índice libro-autor
        var oldLinks = await _readContext.LibroAutores
            .Where(la => la.LibroId == libro.Id)
            .ToListAsync(cancellationToken);
        _readContext.LibroAutores.RemoveRange(oldLinks);

        foreach (var autor in libro.Autores)
            await _readContext.LibroAutores.AddAsync(
                new LibroAutorReadModel { LibroId = libro.Id, AutorId = autor.Id }, cancellationToken);

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
