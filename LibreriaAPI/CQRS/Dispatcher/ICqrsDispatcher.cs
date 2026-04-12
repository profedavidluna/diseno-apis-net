using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Dispatcher;

/// <summary>Dispatcher central de CQRS: despacha queries y commands al handler correcto.</summary>
public interface ICqrsDispatcher
{
    Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
    Task<TResponse> CommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
}
