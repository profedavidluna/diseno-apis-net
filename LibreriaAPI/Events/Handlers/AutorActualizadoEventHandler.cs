using LibreriaAPI.Data;
using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un autor es actualizado en la BD de escritura.
/// También actualiza la info desnormalizada del autor en todos los LibroReadModel afectados.
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

        // Actualizar la info desnormalizada del autor en todos los libros afectados
        var libros = await _readContext.Libros.ToListAsync(cancellationToken);
        foreach (var libro in libros)
        {
            var autores = JsonSerializer.Deserialize<List<AutorDto>>(libro.AutoresJson)
                          ?? [];

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
