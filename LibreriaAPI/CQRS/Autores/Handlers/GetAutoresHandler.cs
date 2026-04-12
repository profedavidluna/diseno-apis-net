using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Autores.Handlers;

public sealed class GetAutoresHandler : IQueryHandler<GetAutoresQuery, IEnumerable<AutorDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAutoresHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<AutorDto>> HandleAsync(GetAutoresQuery query, CancellationToken ct = default)
        => await _uow.Autores.GetAllAsync();
}
