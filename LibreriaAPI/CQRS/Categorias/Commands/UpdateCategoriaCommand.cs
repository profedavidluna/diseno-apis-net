using LibreriaAPI.CQRS.Abstractions;
using LibreriaAPI.DTOs;

namespace LibreriaAPI.CQRS.Categorias.Commands;

public sealed record UpdateCategoriaCommand(int Id, string Nombre, string? Descripcion) : ICommand<CategoriaDto?>;
