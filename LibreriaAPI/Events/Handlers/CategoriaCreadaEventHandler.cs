using LibreriaAPI.Data;
using LibreriaAPI.Events;
using LibreriaAPI.Models.ReadModels;
using MediatR;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando una categoría es creada en la BD de escritura.
/// </summary>
public class CategoriaCreadaEventHandler : INotificationHandler<CategoriaCreadaEvent>
{
    private readonly LibreriaReadContext _readContext;

    public CategoriaCreadaEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(CategoriaCreadaEvent notification, CancellationToken cancellationToken)
    {
        var categoria = notification.Categoria;
        var readModel = new CategoriaReadModel
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion
        };

        await _readContext.Categorias.AddAsync(readModel, cancellationToken);
        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
