using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Commands;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Autores.Handlers;

public sealed class CreateAutorHandler : ICommandHandler<CreateAutorCommand, AutorDto>
{
    private readonly IUnitOfWork _uow;

    public CreateAutorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<AutorDto> HandleAsync(CreateAutorCommand command, CancellationToken ct = default)
    {
        var autor = new Autor
        {
            Nombre = command.Nombre,
            Apellido = command.Apellido,
            Biografia = command.Biografia
        };

        await _uow.Autores.AddAsync(autor);
        await _uow.SaveChangesAsync(ct);

        return new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
