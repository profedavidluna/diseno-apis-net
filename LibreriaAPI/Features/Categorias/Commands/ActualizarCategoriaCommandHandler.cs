using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class ActualizarCategoriaCommandHandler : IRequestHandler<ActualizarCategoriaCommand, bool>
{
    private readonly ICategoriasRepository _repository;
    private readonly IPublisher _publisher;

    public ActualizarCategoriaCommandHandler(ICategoriasRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(ActualizarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetByIdAsync(request.Id);
        if (categoria is null) return false;

        categoria.Nombre = request.Nombre;
        categoria.Descripcion = request.Descripcion;

        await _repository.SaveChangesAsync();

        var categoriaDto = new CategoriaDto(categoria.Id, categoria.Nombre, categoria.Descripcion);
        await _publisher.Publish(new CategoriaActualizadaEvent(categoriaDto), cancellationToken);

        return true;
    }
}
