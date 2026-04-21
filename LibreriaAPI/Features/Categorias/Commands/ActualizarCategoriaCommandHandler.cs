using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class ActualizarCategoriaCommandHandler : IRequestHandler<ActualizarCategoriaCommand, bool>
{
    private readonly ICategoriasRepository _repository;

    public ActualizarCategoriaCommandHandler(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ActualizarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetByIdAsync(request.Id);
        if (categoria is null) return false;

        categoria.Nombre = request.Nombre;
        categoria.Descripcion = request.Descripcion;

        await _repository.SaveChangesAsync();
        return true;
    }
}
