using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un autor es eliminado de la BD de escritura.
/// También elimina la info del autor de todos los LibroReadModel afectados.
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

        // Eliminar la info del autor de todos los libros afectados
        var libros = await _readContext.Libros.ToListAsync(cancellationToken);
        foreach (var libro in libros)
        {
            var autores = JsonSerializer.Deserialize<List<AutorDto>>(libro.AutoresJson)
                          ?? [];

            if (autores.RemoveAll(a => a.Id == notification.AutorId) > 0)
                libro.AutoresJson = JsonSerializer.Serialize(autores);
        }

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
