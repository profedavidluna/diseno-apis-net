namespace LibreriaAPI.CQRS.Abstractions;

/// <summary>Marca una clase como un comando (escritura) que devuelve <typeparamref name="TResponse"/>.</summary>
public interface ICommand<TResponse> { }
