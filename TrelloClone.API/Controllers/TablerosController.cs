using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Infrastructure.Data;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TablerosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TablerosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TableroDto>>> GetAll()
    {
        var tableros = await _context.Tableros
            .Include(t => t.Usuario)
            .Include(t => t.Listas)
                .ThenInclude(l => l.Tarjetas)
                    .ThenInclude(tar => tar.AsignadoA)
            .ToListAsync();

        var result = tableros.Select(t => new TableroDto
        {
            Id = t.Id,
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            Color = t.Color,
            FechaCreacion = t.FechaCreacion,
            UsuarioId = t.UsuarioId,
            NombreUsuario = t.Usuario.Nombre,
            Listas = t.Listas.OrderBy(l => l.Orden).Select(l => new ListaConTarjetasDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Orden = l.Orden,
                Tarjetas = l.Tarjetas.OrderBy(tar => tar.Orden).Select(tar => new TarjetaDto
                {
                    Id = tar.Id,
                    Titulo = tar.Titulo,
                    Descripcion = tar.Descripcion,
                    Prioridad = tar.Prioridad,
                    Estado = tar.Estado,
                    Orden = tar.Orden,
                    FechaCreacion = tar.FechaCreacion,
                    FechaVencimiento = tar.FechaVencimiento,
                    ListaId = tar.ListaId,
                    NombreLista = l.Titulo,
                    AsignadoAId = tar.AsignadoAId,
                    NombreAsignado = tar.AsignadoA?.Nombre
                }).ToList()
            }).ToList()
        }).ToList();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TableroDto>> GetById(int id)
    {
        var tablero = await _context.Tableros
            .Include(t => t.Usuario)
            .Include(t => t.Listas)
                .ThenInclude(l => l.Tarjetas)
                    .ThenInclude(tar => tar.AsignadoA)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tablero == null)
            return NotFound();

        var result = new TableroDto
        {
            Id = tablero.Id,
            Titulo = tablero.Titulo,
            Descripcion = tablero.Descripcion,
            Color = tablero.Color,
            FechaCreacion = tablero.FechaCreacion,
            UsuarioId = tablero.UsuarioId,
            NombreUsuario = tablero.Usuario.Nombre,
            Listas = tablero.Listas.OrderBy(l => l.Orden).Select(l => new ListaConTarjetasDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Orden = l.Orden,
                Tarjetas = l.Tarjetas.OrderBy(tar => tar.Orden).Select(tar => new TarjetaDto
                {
                    Id = tar.Id,
                    Titulo = tar.Titulo,
                    Descripcion = tar.Descripcion,
                    Prioridad = tar.Prioridad,
                    Estado = tar.Estado,
                    Orden = tar.Orden,
                    FechaCreacion = tar.FechaCreacion,
                    FechaVencimiento = tar.FechaVencimiento,
                    ListaId = tar.ListaId,
                    NombreLista = l.Titulo,
                    AsignadoAId = tar.AsignadoAId,
                    NombreAsignado = tar.AsignadoA?.Nombre
                }).ToList()
            }).ToList()
        };

        return Ok(result);
    }
}