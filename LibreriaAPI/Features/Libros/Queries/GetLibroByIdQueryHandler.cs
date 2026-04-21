using LibreriaAPI.DTOs;
using LibreriaAPI.Repositories;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public class GetLibroByIdQueryHandler : IRequestHandler<GetLibroByIdQuery, LibroDto?>
{
    private readonly ILibrosRepository _repository;

    public GetLibroByIdQueryHandler(ILibrosRepository repository)
    {
        _repository = repository;
    }

    public async Task<LibroDto?> Handle(GetLibroByIdQuery request, CancellationToken cancellationToken)
        => await _repository.GetByIdAsync(request.Id);
}
