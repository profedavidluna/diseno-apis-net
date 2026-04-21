using LibreriaAPI.DTOs;
using LibreriaAPI.Events;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class ActualizarAutorCommandHandler : IRequestHandler<ActualizarAutorCommand, bool>
{
    private readonly IAutoresRepository _repository;
    private readonly IPublisher _publisher;

    public ActualizarAutorCommandHandler(IAutoresRepository repository, IPublisher publisher)
    {
        _repository = repository;
        _publisher = publisher;
    }

    public async Task<bool> Handle(ActualizarAutorCommand request, CancellationToken cancellationToken)
    {
        var autor = await _repository.GetByIdAsync(request.Id);
        if (autor is null) return false;

        autor.Nombre = request.Nombre;
        autor.Apellido = request.Apellido;
        autor.Biografia = request.Biografia;

        await _repository.SaveChangesAsync();

        var autorDto = new AutorDto(autor.Id, autor.Nombre, autor.Apellido, autor.Biografia);
        await _publisher.Publish(new AutorActualizadoEvent(autorDto), cancellationToken);

        return true;
    }
}
