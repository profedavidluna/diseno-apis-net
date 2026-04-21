using LibreriaAPI.Events;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class EliminarAutorCommandHandler : IRequestHandler<EliminarAutorCommand, bool>
{
    private readonly IAutoresRepository _repository;
    private readonly IPublisher _publisher;

    public EliminarAutorCommandHandler(IAutoresRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(EliminarAutorCommand request, CancellationToken cancellationToken)
    {
        var autor = await _repository.GetByIdAsync(request.Id);
        if (autor is null) return false;

        _repository.Remove(autor);
        await _repository.SaveChangesAsync();

        await _publisher.Publish(new AutorEliminadoEvent(request.Id), cancellationToken);
        return true;
    }
}
