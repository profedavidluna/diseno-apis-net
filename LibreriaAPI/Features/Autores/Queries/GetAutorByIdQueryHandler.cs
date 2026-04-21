using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Queries;

public class GetAutorByIdQueryHandler : IRequestHandler<GetAutorByIdQuery, AutorDto?>
{
    private readonly IAutoresRepository _repository;

    public GetAutorByIdQueryHandler(IAutoresRepository repository)
    {
        _repository = repository;
    }

    public async Task<AutorDto?> Handle(GetAutorByIdQuery request, CancellationToken cancellationToken)
    {
        var autor = await _repository.GetByIdAsync(request.Id);
        if (autor is null) return null;
        return new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
