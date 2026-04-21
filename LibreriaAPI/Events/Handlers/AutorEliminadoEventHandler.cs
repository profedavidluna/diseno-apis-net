using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un autor es eliminado de la BD de escritura.
/// También elimina la info del autor de los LibroReadModel afectados, usando la tabla
/// de índice LibroAutorReadModel para localizar solo los libros necesarios.
/// </summary>
public class AutorEliminadoEventHandler : INotificationHandler<AutorEliminadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public AutorEliminadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(AutorEliminadoEvent notification, CancellationToken cancellationToken)
    {
        // Eliminar AutorReadModel
        var readModel = await _readContext.Autores.FindAsync([notification.AutorId], cancellationToken);
        if (readModel is not null)
            _readContext.Autores.Remove(readModel);

        // Localizar eficientemente solo los libros que tienen este autor usando el índice
        var libroIds = await _readContext.LibroAutores
            .Where(la => la.AutorId == notification.AutorId)
            .Select(la => la.LibroId)
            .ToListAsync(cancellationToken);

        var librosAfectados = await _readContext.Libros
            .Where(l => libroIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        foreach (var libro in librosAfectados)
        {
            var autores = JsonSerializer.Deserialize<List<AutorDto>>(libro.AutoresJson) ?? [];
            if (autores.RemoveAll(a => a.Id == notification.AutorId) > 0)
                libro.AutoresJson = JsonSerializer.Serialize(autores);
        }

        // Eliminar las entradas del índice
        var links = await _readContext.LibroAutores
            .Where(la => la.AutorId == notification.AutorId)
            .ToListAsync(cancellationToken);
        _readContext.LibroAutores.RemoveRange(links);

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
