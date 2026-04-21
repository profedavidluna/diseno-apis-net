using LibreriaAPI.Data;
using LibreriaAPI.Events;
using LibreriaAPI.Models.ReadModels;
using MediatR;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un libro es creado en la BD de escritura.
/// </summary>
public class LibroCreadoEventHandler : INotificationHandler<LibroCreadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public LibroCreadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(LibroCreadoEvent notification, CancellationToken cancellationToken)
    {
        var libro = notification.Libro;
        var readModel = new LibroReadModel
        {
            Id = libro.Id,
            Titulo = libro.Titulo,
            Descripcion = libro.Descripcion,
            ISBN = libro.ISBN,
            AnioPublicacion = libro.AnioPublicacion,
            CategoriaId = libro.CategoriaId,
            CategoriaNombre = libro.CategoriaNombre,
            AutoresJson = JsonSerializer.Serialize(libro.Autores)
        };

        await _readContext.Libros.AddAsync(readModel, cancellationToken);
        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
