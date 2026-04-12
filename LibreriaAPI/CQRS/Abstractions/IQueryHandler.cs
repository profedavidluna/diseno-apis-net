namespace LibreriaAPI.CQRS.Abstractions;

/// <summary>Contrato para el handler que procesa una consulta de tipo <typeparamref name="TQuery"/>.</summary>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken ct = default);
}
