using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Autores.Queries;

public record GetAutorByIdQuery(int Id) : IRequest<AutorDto?>;
