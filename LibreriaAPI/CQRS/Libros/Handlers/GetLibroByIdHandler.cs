using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class GetLibroByIdHandler : IQueryHandler<GetLibroByIdQuery, LibroDto?>
{
    private readonly IUnitOfWork _uow;

    public GetLibroByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<LibroDto?> HandleAsync(GetLibroByIdQuery query, CancellationToken ct = default)
        => await _uow.Libros.GetByIdAsync(query.Id);
}
