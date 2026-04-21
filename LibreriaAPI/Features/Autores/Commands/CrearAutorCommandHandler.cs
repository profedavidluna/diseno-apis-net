using LibreriaAPI.DTOs;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class CrearAutorCommandHandler : IRequestHandler<CrearAutorCommand, AutorDto>
{
    private readonly IAutoresRepository _repository;

    public CrearAutorCommandHandler(IAutoresRepository repository)
    {
        _repository = repository;
    }

    public async Task<AutorDto> Handle(CrearAutorCommand request, CancellationToken cancellationToken)
    {
        var autor = new Autor
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Biografia = request.Biografia
        };

        await _repository.AddAsync(autor);
        await _repository.SaveChangesAsync();

        return new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
    }
}
