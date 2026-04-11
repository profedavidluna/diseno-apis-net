using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Categorias.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Categorias.Handlers;

public sealed class GetCategoriaByIdHandler : IQueryHandler<GetCategoriaByIdQuery, CategoriaDto?>
{
    private readonly IUnitOfWork _uow;

    public GetCategoriaByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CategoriaDto?> HandleAsync(GetCategoriaByIdQuery query, CancellationToken ct = default)
    {
        var categoria = await _uow.Categorias.GetByIdAsync(query.Id);
        if (categoria is null) return null;
        return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
