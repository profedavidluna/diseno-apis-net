using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.CQRS.Libros.Commands;
using LibreriaAPI.Models;
using LibreriaAPI.UnitOfWork;

namespace LibreriaAPI.CQRS.Libros.Handlers;

public sealed class UpdateLibroHandler : ICommandHandler<UpdateLibroCommand, UpdateLibroResult>
{
    private readonly IUnitOfWork _uow;

    public UpdateLibroHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<UpdateLibroResult> HandleAsync(UpdateLibroCommand command, CancellationToken ct = default)
    {
        var libro = await _uow.Libros.GetWithAutoresAsync(command.Id);
        if (libro is null)
            return new UpdateLibroResult(LibroNoEncontrado: true, CategoriaNoEncontrada: false, AutoresInvalidos: false);

        var categoria = await _uow.Libros.GetCategoriaAsync(command.CategoriaId);
        if (categoria is null)
            return new UpdateLibroResult(LibroNoEncontrado: false, CategoriaNoEncontrada: true, AutoresInvalidos: false);

        var autores = await _uow.Libros.GetAutoresByIdsAsync(command.AutoresIds);
        if (autores.Count != command.AutoresIds.Count())
            return new UpdateLibroResult(LibroNoEncontrado: false, CategoriaNoEncontrada: false, AutoresInvalidos: true);

        libro.Titulo = command.Titulo;
        libro.Descripcion = command.Descripcion;
        libro.ISBN = command.ISBN;
        libro.AnioPublicacion = command.AnioPublicacion;
        libro.CategoriaId = command.CategoriaId;

        _uow.Libros.RemoveLibroAutores(libro.LibroAutores);
        foreach (var autor in autores)
        {
            await _uow.Libros.AddLibroAutorAsync(new LibroAutor { LibroId = libro.Id, AutorId = autor.Id });
        }

        await _uow.SaveChangesAsync(ct);
        return new UpdateLibroResult(LibroNoEncontrado: false, CategoriaNoEncontrada: false, AutoresInvalidos: false);
    }
}
