namespace LibreriaAPI.CQRS.Abstractions;

/// <summary>Contrato para el handler que procesa un comando de tipo <typeparamref name="TCommand"/>.</summary>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct = default);
}
