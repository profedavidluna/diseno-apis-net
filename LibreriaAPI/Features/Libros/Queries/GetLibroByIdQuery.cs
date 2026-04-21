using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Libros.Queries;

public record GetLibroByIdQuery(int Id) : IRequest<LibroDto?>;
