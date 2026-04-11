using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Categorias.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Categorias.Handlers;

public sealed class GetCategoriasHandler : IQueryHandler<GetCategoriasQuery, IEnumerable<CategoriaDto>>
{
    private readonly IUnitOfWork _uow;

    public GetCategoriasHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CategoriaDto>> HandleAsync(GetCategoriasQuery query, CancellationToken ct = default)
        => await _uow.Categorias.GetAllAsync();
}
