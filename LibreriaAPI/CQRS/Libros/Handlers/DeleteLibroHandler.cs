using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Commands;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class DeleteLibroHandler : ICommandHandler<DeleteLibroCommand, bool>
{
    private readonly IUnitOfWork _uow;

    public DeleteLibroHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<bool> HandleAsync(DeleteLibroCommand command, CancellationToken ct = default)
    {
        var libro = await _uow.Libros.GetWithAutoresAsync(command.Id);
        if (libro is null) return false;

        _uow.Libros.RemoveLibroAutores(libro.LibroAutores);
        _uow.Libros.Remove(libro);
        await _uow.SaveChangesAsync(ct);
        return true;
    }
}
