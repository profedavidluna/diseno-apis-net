using Asp.Versioning;
using LibreriaAPI.Features.Autores.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

/// <summary>
/// Gestión de autores — Versión 2.
/// Hereda todos los endpoints de <see cref="AutoresController"/> (V1) y agrega
/// el endpoint GET /{id}/nombre-completo que devuelve el nombre completo del autor.
/// </summary>
[ApiVersion("2.0")]
public class AutoresV2Controller : AutoresController
{
    public AutoresV2Controller(IMediator mediator) : base(mediator) { }

    /// <summary>[V2] Devuelve el nombre completo (Nombre + Apellido) de un autor</summary>
    [HttpGet("{id:int}/nombre-completo")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetNombreCompleto(int id)
    {
        var autor = await _mediator.Send(new GetAutorByIdQuery(id));

        if (autor is null)
            return NotFound();

        return Ok($"{autor.Nombre} {autor.Apellido}");
    }
}
