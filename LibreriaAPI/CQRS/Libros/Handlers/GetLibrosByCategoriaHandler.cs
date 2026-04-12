using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class GetLibrosByCategoriaHandler : IQueryHandler<GetLibrosByCategoriaQuery, IEnumerable<LibroDto>>
{
    private readonly IUnitOfWork _uow;

    public GetLibrosByCategoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LibroDto>> HandleAsync(GetLibrosByCategoriaQuery query, CancellationToken ct = default)
        => await _uow.Libros.GetByCategoriaAsync(query.CategoriaId);
}
