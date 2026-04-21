using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public class EliminarLibroCommandHandler : IRequestHandler<EliminarLibroCommand, bool>
{
    private readonly ILibrosRepository _repository;

    public EliminarLibroCommandHandler(ILibrosRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(EliminarLibroCommand request, CancellationToken cancellationToken)
    {
        var libro = await _repository.GetWithAutoresAsync(request.Id);
        if (libro is null) return false;

        _repository.RemoveLibroAutores(libro.LibroAutores);
        _repository.Remove(libro);
        await _repository.SaveChangesAsync();
        return true;
    }
}
