using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Commands;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Autores.Handlers;

public sealed class DeleteAutorHandler : ICommandHandler<DeleteAutorCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteAutorHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> HandleAsync(DeleteAutorCommand command, CancellationToken ct = default)
    {
        var autor = await _uow.Autores.GetByIdAsync(command.Id);
        if (autor is null) return false;

        _uow.Autores.Remove(autor);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
