using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Commands;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class CreateLibroHandler : ICommandHandler<CreateLibroCommand, CreateLibroResult>
{
    private readonly IUnitOfWork _uow;

    public CreateLibroHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<CreateLibroResult> HandleAsync(CreateLibroCommand command, CancellationToken ct = default)
    {
        var categoria = await _uow.Libros.GetCategoriaAsync(command.CategoriaId);
        if (categoria is null)
            return new CreateLibroResult(null, CategoriaNoEncontrada: true, AutoresInvalidos: false);

        var autores = await _uow.Libros.GetAutoresByIdsAsync(command.AutoresIds);
        if (autores.Count != command.AutoresIds.Count())
            return new CreateLibroResult(null, CategoriaNoEncontrada: false, AutoresInvalidos: true);

        var libro = new Libro
        {
            Titulo = command.Titulo,
            Descripcion = command.Descripcion,
            ISBN = command.ISBN,
            AnioPublicacion = command.AnioPublicacion,
            CategoriaId = command.CategoriaId
        };

        await _uow.Libros.AddAsync(libro);
        await _uow.SaveChangesAsync(ct);

        foreach (var autor in autores)
        {
            await _uow.Libros.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }
        await _uow.SaveChangesAsync(ct);

        var libroDto = await _uow.Libros.GetByIdAsync(libro.Id);
        return new CreateLibroResult(libroDto, CategoriaNoEncontrada: false, AutoresInvalidos: false);
    }
}
