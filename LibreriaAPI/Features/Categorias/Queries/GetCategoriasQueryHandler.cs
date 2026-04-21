using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Queries;

public class GetCategoriasQueryHandler : IRequestHandler<GetCategoriasQuery, IEnumerable<CategoriaDto>>
{
    private readonly ICategoriasRepository _repository;

    public GetCategoriasQueryHandler(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoriaDto>> Handle(GetCategoriasQuery request, CancellationToken cancellationToken)
        => await _repository.GetAllAsync();
}
