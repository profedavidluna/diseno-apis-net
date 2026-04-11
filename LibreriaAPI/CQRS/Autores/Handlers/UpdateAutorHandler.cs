using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Commands;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Autores.Handlers;

public sealed class UpdateAutorHandler : ICommandHandler<UpdateAutorCommand, AutorDto?>
{
    private readonly IUnitOfWork _uow;

    public UpdateAutorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<AutorDto?> HandleAsync(UpdateAutorCommand command, CancellationToken ct = default)
    {
        var autor = await _uow.Autores.GetByIdAsync(command.Id);
        if (autor is null) return null;

        autor.Nombre = command.Nombre;
        autor.Apellido = command.Apellido;
        autor.Biografia = command.Biografia;

        await _uow.SaveChangesAsync(ct);
        return new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
