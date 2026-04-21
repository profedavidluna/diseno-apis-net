using LibreriaAPI.Data;
using LibreriaAPI.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando una categoría es eliminada de la BD de escritura.
/// También limpia el nombre desnormalizado de la categoría en los LibroReadModel afectados.
/// </summary>
public class CategoriaEliminadaEventHandler : INotificationHandler<CategoriaEliminadaEvent>
{
    private readonly LibreriaReadContext _readContext;

    public CategoriaEliminadaEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(CategoriaEliminadaEvent notification, CancellationToken cancellationToken)
    {
        // Eliminar CategoriaReadModel
        var readModel = await _readContext.Categorias.FindAsync([notification.CategoriaId], cancellationToken);
        if (readModel is not null)
            _readContext.Categorias.Remove(readModel);

        // Limpiar el nombre desnormalizado en los libros que referenciaban esta categoría
        var librosAfectados = await _readContext.Libros
            .Where(l => l.CategoriaId == notification.CategoriaId)
            .ToListAsync(cancellationToken);

        foreach (var libro in librosAfectados)
            libro.CategoriaNombre = null;

        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
