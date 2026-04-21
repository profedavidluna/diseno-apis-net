using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using LibreriaAPI.Models;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class CrearAutorCommandHandler : IRequestHandler<CrearAutorCommand, AutorDto>
{
    private readonly IAutoresRepository _repository;
    private readonly IPublisher _publisher;

    public CrearAutorCommandHandler(IAutoresRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
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

        var autorDto = new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
        await _publisher.Publish(new AutorCreadoEvent(autorDto), cancellationToken);

        return autorDto;
    }
}
