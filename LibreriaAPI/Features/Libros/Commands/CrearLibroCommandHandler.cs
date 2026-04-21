using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Commands;

public class CrearLibroCommandHandler : IRequestHandler<CrearLibroCommand, CrearLibroResult>
{
    private readonly ILibrosRepository _repository;

    public CrearLibroCommandHandler(ILibrosRepository repository)
    {
        _repository = repository;
    }

    public async Task<CrearLibroResult> Handle(CrearLibroCommand request, CancellationToken cancellationToken)
    {
        var categoria = await _repository.GetCategoriaAsync(request.CategoriaId);
        if (categoria is null)
            return new CrearLibroResult(null, $"Categoría con Id {request.CategoriaId} no encontrada.");

        var autores = await _repository.GetAutoresByIdsAsync(request.AutoresIds);
        if (autores.Count != request.AutoresIds.Count())
            return new CrearLibroResult(null, "Uno o más autores no fueron encontrados.");

        var libro = new Libro
        {
            Titulo = request.Titulo,
            Descripcion = request.Descripcion,
            ISBN = request.ISBN,
            AnioPublicacion = request.AnioPublicacion,
            CategoriaId = request.CategoriaId
        };

        await _repository.AddAsync(libro);
        await _repository.SaveChangesAsync();

        foreach (var autor in autores)
        {
            await _repository.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _repository.SaveChangesAsync();

        var libroDto = await _repository.GetByIdAsync(libro.Id);
        return new CrearLibroResult(libroDto, null);
    }
}
