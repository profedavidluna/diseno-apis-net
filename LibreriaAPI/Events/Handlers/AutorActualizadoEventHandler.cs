using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un autor es actualizado en la BD de escritura.
/// También actualiza la info desnormalizada del autor en todos los LibroReadModel afectados,
/// usando la tabla de índice LibroAutorReadModel para localizar solo los libros necesarios.
/// </summary>
public class AutorActualizadoEventHandler : INotificationHandler<AutorActualizadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public AutorActualizadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(AutorActualizadoEvent notification, CancellationToken cancellationToken)
    {
        var autor = notification.Autor;

        // Actualizar AutorReadModel
        var readModel = await _readContext.Autores.FindAsync([autor.Id], cancellationToken);
        if (readModel is not null)
        {
            readModel.Nombre = autor.Nombre;
            readModel.Apellido = autor.Apellido;
            readModel.Biografia = autor.Biografia;
        }

        // Localizar eficientemente solo los libros que tienen este autor usando el índice
        var libroIds = await _readContext.LibroAutores
            .Where(la => la.AutorId == autor.Id)
            .Select(la => la.LibroId)
            .ToListAsync(cancellationToken);

        var librosAfectados = await _readContext.Libros
            .Where(l => libroIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        foreach (var libro in librosAfectados)
        {
            var autores = JsonSerializer.Deserialize<List<AutorDto>>(libro.AutoresJson) ?? [];
            var index = autores.FindIndex(a => a.Id == autor.Id);
            if (index >= 0)
            {
                autores[index] = autor;
                libro.AutoresJson = JsonSerializer.Serialize(autores);
            }
        }

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
