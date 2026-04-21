using LibreriaAPI.Data;
using LibreriaAPI.Events;
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

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
