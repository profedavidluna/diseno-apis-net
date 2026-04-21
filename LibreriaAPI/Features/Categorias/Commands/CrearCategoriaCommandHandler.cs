using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class CrearCategoriaCommandHandler : IRequestHandler<CrearCategoriaCommand, CategoriaDto>
{
    private readonly ICategoriasRepository _repository;
    private readonly IPublisher _publisher;

    public CrearCategoriaCommandHandler(ICategoriasRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
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

        var categoriaDto = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        await _publisher.Publish(new CategoriaCreadaEvent(categoriaDto), cancellationToken);

        return categoriaDto;
    }
}
