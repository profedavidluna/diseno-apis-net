using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class GetLibrosHandler : IQueryHandler<GetLibrosQuery, IEnumerable<LibroDto>>
{
    private readonly IUnitOfWork _uow;

    public GetLibrosHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<LibroDto>> HandleAsync(GetLibrosQuery query, CancellationToken ct = default)
        => await _uow.Libros.GetAllAsync();
}
