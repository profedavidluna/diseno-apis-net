using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class EliminarCategoriaCommandHandler : IRequestHandler<EliminarCategoriaCommand, bool>
{
    private readonly ICategoriasRepository _repository;

    public EliminarCategoriaCommandHandler(ICategoriasRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(EliminarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetByIdAsync(request.Id);
        if (categoria is null) return false;

        _repository.Remove(categoria);
        await _repository.SaveChangesAsync();
        return true;
    }
}
