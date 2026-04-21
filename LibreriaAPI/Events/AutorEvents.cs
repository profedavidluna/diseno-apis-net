using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Events;

/// <summary>Se dispara cuando un autor es creado en la BD de escritura.</summary>
public record AutorCreadoEvent(AutorDto Autor) : INotification;

/// <summary>Se dispara cuando un autor es actualizado en la BD de escritura.</summary>
public record AutorActualizadoEvent(AutorDto Autor) : INotification;

/// <summary>Se dispara cuando un autor es eliminado de la BD de escritura.</summary>
public record AutorEliminadoEvent(int AutorId) : INotification;
