using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Categorias.Commands;
using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Categorias.Handlers;

public sealed class CreateCategoriaHandler : ICommandHandler<CreateCategoriaCommand, CategoriaDto>
{
    private readonly IUnitOfWork _uow;

    public CreateCategoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CategoriaDto> HandleAsync(CreateCategoriaCommand command, CancellationToken ct = default)
    {
        var categoria = new Categoria
        {
            Nombre = command.Nombre,
            Descripcion = command.Descripcion
        };

        await _uow.Categorias.AddAsync(categoria);
        await _uow.SaveChangesAsync(ct);

        return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
