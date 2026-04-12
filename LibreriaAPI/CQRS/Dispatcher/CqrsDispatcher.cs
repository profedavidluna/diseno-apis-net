using LibreriaAPI.CQRS.Abstractions;

namespace LibreriaAPI.CQRS.Dispatcher;

/// <summary>
/// Implementación del dispatcher CQRS.
/// Resuelve el handler adecuado desde el contenedor de DI usando reflexión + dynamic,
/// lo que permite enseñar el patrón sin depender de librerías externas como MediatR.
/// </summary>
public sealed class CqrsDispatcher : ICqrsDispatcher
{
    private readonly IServiceProvider _sp;

    public CqrsDispatcher(IServiceProvider sp) => _sp = sp;

    public Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
    {
        // Construye el tipo concreto IQueryHandler<TConcreteQuery, TResponse> en tiempo de ejecución
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResponse));
        dynamic handler = _sp.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)query, ct);
    }

    public Task<TResponse> CommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
    {
        // Construye el tipo concreto ICommandHandler<TConcreteCommand, TResponse> en tiempo de ejecución
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResponse));
        dynamic handler = _sp.GetRequiredService(handlerType);
        return handler.HandleAsync((dynamic)command, ct);
    }
}
