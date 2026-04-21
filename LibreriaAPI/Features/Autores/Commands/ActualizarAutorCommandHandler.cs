using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class ActualizarAutorCommandHandler : IRequestHandler<ActualizarAutorCommand, bool>
{
    private readonly IAutoresRepository _repository;

    public ActualizarAutorCommandHandler(IAutoresRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ActualizarAutorCommand request, CancellationToken cancellationToken)
    {
        var autor = await _repository.GetByIdAsync(request.Id);
        if (autor is null) return false;

        autor.Nombre = request.Nombre;
        autor.Apellido = request.Apellido;
        autor.Biografia = request.Biografia;

        await _repository.SaveChangesAsync();
        return true;
    }
}
