using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrelloClone.Application.DTOs.Admin;
using TrelloClone.Infraestructure.Data;

namespace TrelloClone.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasDto>> GetEstadisticas()
    {
        var totalUsuarios = await _context.Usuarios.CountAsync();
        var totalTableros = await _context.Tableros.CountAsync();
        var totalTarjetas = await _context.Tarjetas.CountAsync();

        var tarjetasPorEstado = await _context.Tarjetas
            .GroupBy(t => t.Estado)
            .Select(g => new TarjetasPorEstadoDto
            {
                Estado = g.Key,
                Cantidad = g.Count()
            })
            .ToListAsync();

        var tarjetasPorPrioridad = await _context.Tarjetas
            .GroupBy(t => t.Prioridad)
            .Select(g => new TarjetasPorPrioridadDto
            {
                Prioridad = g.Key,
                Cantidad = g.Count()
            })
            .ToListAsync();

        var estadisticas = new EstadisticasDto
        {
            TotalUsuarios = totalUsuarios,
            TotalTableros = totalTableros,
            TotalTarjetas = totalTarjetas,
            TarjetasPorEstado = tarjetasPorEstado,
            TarjetasPorPrioridad = tarjetasPorPrioridad
        };

        return Ok(estadisticas);
    }
}