using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Autores.Queries;

public record GetAutoresQuery : IRequest<IEnumerable<AutorDto>>;
