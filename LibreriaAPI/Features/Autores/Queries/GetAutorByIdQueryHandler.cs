using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Queries;

public class GetAutorByIdQueryHandler : IRequestHandler<GetAutorByIdQuery, AutorDto?>
{
    private readonly IAutoresReadRepository _repository;

    public GetAutorByIdQueryHandler(IAutoresReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<AutorDto?> Handle(GetAutorByIdQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id);
}
