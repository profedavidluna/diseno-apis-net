using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Autores.Commands;

public sealed record DeleteAutorCommand(int Id) : ICommand<bool>;
