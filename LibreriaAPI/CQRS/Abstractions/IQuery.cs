namespace LibreriaAPI.CQRS.Abstractions;

/// <summary>Marca una clase como una consulta (read-only) que devuelve <typeparamref name="TResponse"/>.</summary>
public interface IQuery<TResponse> { }
