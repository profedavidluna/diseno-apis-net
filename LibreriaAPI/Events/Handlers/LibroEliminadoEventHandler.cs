using LibreriaAPI.Data;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un libro es eliminado de la BD de escritura.
/// </summary>
public class LibroEliminadoEventHandler : INotificationHandler<LibroEliminadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public LibroEliminadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(LibroEliminadoEvent notification, CancellationToken cancellationToken)
    {
        var readModel = await _readContext.Libros.FindAsync([notification.LibroId], cancellationToken);

        if (readModel is null) return;

        _readContext.Libros.Remove(readModel);

        // Limpiar la tabla de índice libro-autor
        var links = await _readContext.LibroAutores
            .Where(la => la.LibroId == notification.LibroId)
            .ToListAsync(cancellationToken);
        _readContext.LibroAutores.RemoveRange(links);

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
