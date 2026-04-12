using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Libros.Commands;

public sealed record DeleteLibroCommand(int Id) : ICommand<bool>;
