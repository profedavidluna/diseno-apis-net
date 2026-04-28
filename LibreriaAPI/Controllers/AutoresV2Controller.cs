using Asp.Versioning;
using LibreriaAPI.DTOs;
using LibreriaAPI.Features.Autores.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibreriaAPI.Controllers;

/// <summary>
/// Controlador de Autores v2. Hereda todos los endpoints de v1 y sobreescribe
/// el listado general para devolver el nombre completo del autor.
/// </summary>
[ApiVersion("2.0")]
public class AutoresV2Controller : AutoresController
{
    public AutoresV2Controller(IMediator mediator) : base(mediator) { }

    /// <summary>Obtiene todos los autores con nombre completo (v2)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorV2Dto>), StatusCodes.Status200OK)]
    public override async Task<IActionResult> GetAutores()
    {
        var autores = await _mediator.Send(new GetAutoresQuery());
        var result = autores.Select(a => new AutorV2Dto(
            a.Id,
            a.Nombre,
            a.Apellido,
            a.Biografia,
            $"{a.Nombre} {a.Apellido}"
        ));
        return Ok(result);
    }
}
