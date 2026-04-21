using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class CrearCategoriaCommandHandler : IRequestHandler<CrearCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriasRepository _repository;

    public CrearCategoriaCommandHandler(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    public async Task<CategoriaDto> Handle(CrearCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = new Categoria
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion
        };

        await _repository.AddAsync(categoria);
        await _repository.SaveChangesAsync();

        return new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
    }
}
