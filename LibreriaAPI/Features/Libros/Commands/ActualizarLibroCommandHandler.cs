using LibreriaAPI.Events;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public class ActualizarLibroCommandHandler : IRequestHandler<ActualizarLibroCommand, ActualizarLibroResult>
{
    private readonly ILibrosRepository _repository;
    private readonly IPublisher _publisher;

    public ActualizarLibroCommandHandler(ILibrosRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<ActualizarLibroResult> Handle(ActualizarLibroCommand request, CancellationToken cancellationToken)
    {
        var libro = await _repository.GetWithAutoresAsync(request.Id);
        if (libro is null)
            return new ActualizarLibroResult(false, null);

        var categoria = await _repository.GetCategoriaAsync(request.CategoriaId);
        if (categoria is null)
            return new ActualizarLibroResult(true, $"Categoría con Id {request.CategoriaId} no encontrada.");

        var autores = await _repository.GetAutoresByIdsAsync(request.AutoresIds);
        if (autores.Count != request.AutoresIds.Count())
            return new ActualizarLibroResult(true, "Uno o más autores no fueron encontrados.");

        libro.Titulo = request.Titulo;
        libro.Descripcion = request.Descripcion;
        libro.ISBN = request.ISBN;
        libro.AnioPublicacion = request.AnioPublicacion;
        libro.CategoriaId = request.CategoriaId;

        _repository.RemoveLibroAutores(libro.LibroAutores);
        await Task.WhenAll(autores.Select(autor =>
            _repository.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id })));

        await _repository.SaveChangesAsync();

        var libroDto = await _repository.GetByIdAsync(request.Id);
        if (libroDto is not null)
            await _publisher.Publish(new LibroActualizadoEvent(libroDto), cancellationToken);

        return new ActualizarLibroResult(true, null);
    }
}
