using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Queries;

public class GetCategoriaByIdQueryHandler : IRequestHandler<GetCategoriaByIdQuery, CategoriaDto?>
{
    private readonly ICategoriasRepository _repository;

    public GetCategoriaByIdQueryHandler(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoriaDto?> Handle(GetCategoriaByIdQuery request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetByIdAsync(request.Id);
        if (categoria is null) return null;
        return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
