using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Queries;

public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto?>
{
    private readonly ICategoriasReadRepository _repository;

    public GetCategoriaByIdQueryHandler(ICategoriasReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoriaDto?> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id);
}
