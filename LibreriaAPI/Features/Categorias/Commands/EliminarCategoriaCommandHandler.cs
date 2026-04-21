using LibreriaAPI.Events;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public class EliminarCategoriaCommandHandler : IRequestHandler<EliminarCategoriaCommand, bool>
{
    private readonly ICategoriasRepository _repository;
    private readonly IPublisher _publisher;

    public EliminarCategoriaCommandHandler(ICategoriasRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(EliminarCategoriaCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetByIdAsync(request.Id);
        if (categoria is null) return false;

        _repository.Remove(categoria);
        await _repository.SaveChangesAsync();

        await _publisher.Publish(new CategoriaEliminadaEvent(request.Id), cancellationToken);
        return true;
    }
}
