using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.Interfaces;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablerosController : ControllerBase
{
    private readonly ITablerosService _tablerosService;

    public TablerosController(ITablerosService tablerosService)
    {
        _tablerosService = tablerosService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TableroDto>>> GetAll()
    {
        var result = await _tablerosService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TableroDto>> Create([FromBody] CrearTableroDTO dto)
    {
        // Obtener el id del usuario autenticado desde el token
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

       dto.UsuarioId = int.Parse(userIdClaim.Value);

        var tablero = await _tablerosService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tablero.Id }, tablero);
    }

    [HttpPost("{tableroId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ListaDTO>> CreateLista(int tableroId, [FromBody] CreateListaDTO createLista)
    {
        var lista = await _tablerosService.CreateLista(tableroId, createLista);
        return CreatedAtAction(nameof(GetById), new { id = tableroId }, lista);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<TableroDto>> GetById(int id)
    {
        var tablero = await _tablerosService.GetByIdAsync(id);
        if (tablero == null)
            return NotFound();
        return Ok(tablero);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _tablerosService.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = $"Tablero con ID:{id} no encontrado" });
        return NoContent();
    }

}