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

    [HttpGet("{id}")]
    public async Task<ActionResult<TableroDto>> GetById(int id)
    {
        return BadRequest();
        //var tablero = await _context.Tableros
        //    .Include(t => t.Usuario)
        //    .Include(t => t.Listas)
        //        .ThenInclude(l => l.Tarjetas)
        //            .ThenInclude(tar => tar.AsignadoA)
        //    .FirstOrDefaultAsync(t => t.Id == id);

        //if (tablero == null)
        //    return NotFound();

        //var result = new TableroDto
        //{
        //    Id = tablero.Id,
        //    Titulo = tablero.Titulo,
        //    Descripcion = tablero.Descripcion,
        //    Color = tablero.Color,
        //    FechaCreacion = tablero.FechaCreacion,
        //    UsuarioId = tablero.UsuarioId,
        //    NombreUsuario = tablero.Usuario.Nombre,
        //    Listas = tablero.Listas.OrderBy(l => l.Orden).Select(l => new ListaConTarjetasDto
        //    {
        //        Id = l.Id,
        //        Titulo = l.Titulo,
        //        Orden = l.Orden,
        //        Tarjetas = l.Tarjetas.OrderBy(tar => tar.Orden).Select(tar => new TarjetaDto
        //        {
        //            Id = tar.Id,
        //            Titulo = tar.Titulo,
        //            Descripcion = tar.Descripcion,
        //            Prioridad = tar.Prioridad,
        //            Estado = tar.Estado,
        //            Orden = tar.Orden,
        //            FechaCreacion = tar.FechaCreacion,
        //            FechaVencimiento = tar.FechaVencimiento,
        //            ListaId = tar.ListaId,
        //            NombreLista = l.Titulo,
        //            AsignadoAId = tar.AsignadoAId,
        //            NombreAsignado = tar.AsignadoA?.Nombre
        //        }).ToList()
        //    }).ToList()
        //};

        //return Ok(result);
    }
}