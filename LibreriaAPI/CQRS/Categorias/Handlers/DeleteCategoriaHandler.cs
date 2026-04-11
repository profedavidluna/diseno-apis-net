using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Categorias.Commands;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Categorias.Handlers;

public sealed class DeleteCategoriaHandler : ICommandHandler<DeleteCategoriaCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteCategoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> HandleAsync(DeleteCategoriaCommand command, CancellationToken ct = default)
    {
        var categoria = await _uow.Categorias.GetByIdAsync(command.Id);
        if (categoria is null) return false;

        _uow.Categorias.Remove(categoria);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
