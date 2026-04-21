using MediatR;

namespace LibreriaAPI.Features.Categorias.Commands;

public record ActualizarCategoriaCommand(int Id, string Nombre, string? Descripcion) : IRequest<bool>;
