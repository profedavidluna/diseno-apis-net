using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public class GetLibrosPorCategoriaQueryHandler : IRequestHandler<GetLibrosPorCategoriaQuery, IEnumerable<LibroDto>>
{
    private readonly ILibrosRepository _repository;

    public GetLibrosPorCategoriaQueryHandler(ILibrosRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LibroDto>> Handle(GetLibrosPorCategoriaQuery request, CancellationToken cancellationToken)
        => await _repository.GetByCategoriaAsync(request.CategoriaId);
}
