using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Categorias.Commands;

public sealed record CreateCategoriaCommand(string Nombre, string? Descripcion) : ICommand<CategoriaDto>;
