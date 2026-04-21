using LibreriaAPI.Events;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public class EliminarLibroCommandHandler : IRequestHandler<EliminarLibroCommand, bool>
{
    private readonly ILibrosRepository _repository;
    private readonly IPublisher _publisher;

    public EliminarLibroCommandHandler(ILibrosRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(EliminarLibroCommand request, CancellationToken cancellationToken)
    {
        var libro = await _repository.GetWithAutoresAsync(request.Id);
        if (libro is null) return false;

        _repository.RemoveLibroAutores(libro.LibroAutores);
        _repository.Remove(libro);
        await _repository.SaveChangesAsync();

        await _publisher.Publish(new LibroEliminadoEvent(request.Id), cancellationToken);
        return true;
    }
}
