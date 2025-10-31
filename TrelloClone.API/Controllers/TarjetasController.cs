using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TarjetasController : ControllerBase
{
    private readonly ITarjetaService _tarjetaService;

    public TarjetasController(ITarjetaService tarjetaService)
    {
        _tarjetaService = tarjetaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TarjetaDto>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? estado = null)
    {
        var tarjetas = await _tarjetaService.GetAllAsync(search, estado);
        return Ok(tarjetas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TarjetaDto>> GetById(int id)
    {
        var tarjeta = await _tarjetaService.GetByIdAsync(id);

        if (tarjeta == null)
            return NotFound(new { message = $"Tarjeta con ID {id} no encontrada" });

        return Ok(tarjeta);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TarjetaDto>> Create([FromBody] CreateTarjetaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tarjeta = await _tarjetaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tarjeta.Id }, tarjeta);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TarjetaDto>> Update(int id, [FromBody] CreateTarjetaDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tarjeta = await _tarjetaService.UpdateAsync(id, dto);

        if (tarjeta == null)
            return NotFound(new { message = $"Tarjeta con ID {id} no encontrada" });

        return Ok(tarjeta);
    }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tarjetaService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = $"Tarjeta con ID {id} no encontrada" });

            return NoContent();
        }

    [HttpPatch("{id}/mover")]
    public async Task<IActionResult> Mover(int id, [FromBody] MoverTarjetaDTO dto)
    {
        var result = await _tarjetaService.MoverTarjetaAsync(id, dto);

        if (!result)
            return NotFound(new { message = $"Tarjeta con ID {id} no encontrada" });

        return NoContent();
    }
}