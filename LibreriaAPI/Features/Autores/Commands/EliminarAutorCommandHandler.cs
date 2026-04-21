using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Commands;

public class EliminarAutorCommandHandler : IRequestHandler<EliminarAutorCommand, bool>
{
    private readonly IAutoresRepository _repository;

    public EliminarAutorCommandHandler(IAutoresRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(EliminarAutorCommand request, CancellationToken cancellationToken)
    {
        var autor = await _repository.GetByIdAsync(request.Id);
        if (autor is null) return false;

        _repository.Remove(autor);
        await _repository.SaveChangesAsync();
        return true;
    }
}
