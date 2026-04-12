using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Categorias.Commands;

public sealed record DeleteCategoriaCommand(int Id) : ICommand<bool>;
