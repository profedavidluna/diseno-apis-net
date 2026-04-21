using LibreriaAPI.Data;
using LibreriaAPI.Events;
using LibreriaAPI.Models.ReadModels;
using MediatR;

namespace LibreriaAPI.Events.Handlers;

/// <summary>
/// Sincroniza la BD de lectura cuando un autor es creado en la BD de escritura.
/// </summary>
public class AutorCreadoEventHandler : INotificationHandler<AutorCreadoEvent>
{
    private readonly LibreriaReadContext _readContext;

    public AutorCreadoEventHandler(LibreriaReadContext readContext)
    {
        _readContext = readContext;
    }

    public async Task Handle(AutorCreadoEvent notification, CancellationToken cancellationToken)
    {
        var autor = notification.Autor;
        var readModel = new AutorReadModel
        {
            Id = autor.Id,
            Nombre = autor.Nombre,
            Apellido = autor.Apellido,
            Biografia = autor.Biografia
        };

        await _readContext.Autores.AddAsync(readModel, cancellationToken);
        await _readContext.SaveChangesAsync(cancellationToken);
    }
}
