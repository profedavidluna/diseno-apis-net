using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Events;

/// <summary>Se dispara cuando un libro es creado en la BD de escritura.</summary>
public record LibroCreadoEvent(LibroDto Libro) : INotification;

/// <summary>Se dispara cuando un libro es actualizado en la BD de escritura.</summary>
public record LibroActualizadoEvent(LibroDto Libro) : INotification;

/// <summary>Se dispara cuando un libro es eliminado de la BD de escritura.</summary>
public record LibroEliminadoEvent(int LibroId) : INotification;
