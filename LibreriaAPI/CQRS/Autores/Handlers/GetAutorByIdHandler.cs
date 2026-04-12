using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Autores.Queries;
using LibreriaAPI.DTOs;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Autores.Handlers;

public sealed class GetAutorByIdHandler : IQueryHandler<GetAutorByIdQuery, AutorDto?>
{
    private readonly IUnitOfWork _uow;

    public GetAutorByIdHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<AutorDto?> HandleAsync(GetAutorByIdQuery query, CancellationToken ct = default)
    {
        var autor = await _uow.Autores.GetByIdAsync(query.Id);
        if (autor is null) return null;
        return new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
