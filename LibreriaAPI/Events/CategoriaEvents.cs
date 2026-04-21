using LibreriaAPI.DTOs;
using MediatR;

namespace LibreriaAPI.Events;

/// <summary>Se dispara cuando una categoría es creada en la BD de escritura.</summary>
public record CategoriaCreadaEvent(CategoriaDto Categoria) : INotification;

/// <summary>Se dispara cuando una categoría es actualizada en la BD de escritura.</summary>
public record CategoriaActualizadaEvent(CategoriaDto Categoria) : INotification;

/// <summary>Se dispara cuando una categoría es eliminada de la BD de escritura.</summary>
public record CategoriaEliminadaEvent(int CategoriaId) : INotification;
