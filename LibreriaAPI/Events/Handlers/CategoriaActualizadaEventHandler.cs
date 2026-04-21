using LibreriaAPI.Data;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando una categoría es actualizada en la BD de escritura.
/// También actualiza el nombre desnormalizado de la categoría en todos los LibroReadModel afectados.
/// </summary>
public class CategoriaActualizadaEventHandler : INotificationHandler<CategoriaActualizadaEvent>
{
    private readonly LibreriaReadContext _readContext;

    public CategoriaActualizadaEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(CategoriaActualizadaEvent notification, CancellationToken cancellationToken)
    {
        var categoria = notification.Categoria;

        // Actualizar CategoriaReadModel
        var readModel = await _readContext.Categorias.FindAsync([categoria.Id], cancellationToken);
        if (readModel is not null)
        {
            readModel.Nombre = categoria.Nombre;
            readModel.Descripcion = categoria.Descripcion;
        }

        // Actualizar el nombre desnormalizado en todos los libros afectados
        var librosAfectados = await _readContext.Libros
            .Where(l => l.CategoriaId == categoria.Id)
            .ToListAsync(cancellationToken);

        foreach (var libro in librosAfectados)
            libro.CategoriaNombre = categoria.Nombre;

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
