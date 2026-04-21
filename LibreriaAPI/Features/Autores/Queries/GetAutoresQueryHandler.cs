using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Autores.Queries;

public class GetAutoresQueryHandler : IRequestHandler<GetAutoresQuery, IEnumerable<AutorDto>>
{
    private readonly IAutoresReadRepository _repository;

    public GetAutoresQueryHandler(IAutoresReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AutorDto>> Handle(GetAutoresQuery request, CancellationToken cancellationToken)
        => await _repository.GetAllAsync();
}
