using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public class GetLibrosQueryHandler : IRequestHandler<GetLibrosQuery, IEnumerable<LibroDto>>
{
    private readonly ILibrosReadRepository _repository;

    public GetLibrosQueryHandler(ILibrosReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LibroDto>> Handle(GetLibrosQuery request, CancellationToken cancellationToken)
        => await _repository.GetAllAsync();
}
