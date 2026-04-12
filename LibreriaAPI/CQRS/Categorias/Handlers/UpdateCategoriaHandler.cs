using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Categorias.Commands;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Categorias.Handlers;

public sealed class UpdateCategoriaHandler : ICommandHandler<UpdateCategoriaCommand, CategoriaDto?>
{
    private readonly IUnitOfWork _uow;

    public UpdateCategoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CategoriaDto?> HandleAsync(UpdateCategoriaCommand command, CancellationToken ct = default)
    {
        var categoria = await _uow.Categorias.GetByIdAsync(command.Id);
        if (categoria is null) return null;

        categoria.Nombre = command.Nombre;
        categoria.Descripcion = command.Descripcion;

        await _uow.SaveChangesAsync(ct);
        return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
