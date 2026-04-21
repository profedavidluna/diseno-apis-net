using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public record CrearCategoriaCommand(string Nombre, string? Descripcion) : IRequest<CategoriaDto>;
