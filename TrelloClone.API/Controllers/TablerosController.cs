using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;
using TrelloClone.Infrastructure.Data;

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
    public async Task<ActionResult<TableroDto>> Create([FromBody] CrearTableroDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        var tablero = await _tablerosService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = tablero.Id }, tablero);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableroDto>> GetById(int id)
    {
        var tablero = await _tablerosService.GetByIdAsync(id);
        if (tablero == null)
            return NotFound();
        return Ok(tablero);
    }

}