using Microsoft.EntityFrameworkCore;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;
using TrelloClone.Domain.Entities;
using TrelloClone.Infrastructure.Data;

namespace TrelloClone.Infrastructure.Services;

public class TarjetaService : ITarjetaService
{
    private readonly ApplicationDbContext _context;

    public TarjetaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TarjetaDto>> GetAllAsync(string? search = null, string? estado = null)
    {
        var query = _context.Tarjetas
            .Include(t => t.Lista)
            .Include(t => t.AsignadoA)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.Titulo.Contains(search) ||
                t.Descripcion.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            query = query.Where(t => t.Estado == estado);
        }

        var tarjetas = await query
            .OrderBy(t => t.Lista.Orden)
            .ThenBy(t => t.Orden)
            .ToListAsync();

        return tarjetas.Select(MapToDto).ToList();
    }

    public async Task<TarjetaDto?> GetByIdAsync(int id)
    {
        var tarjeta = await _context.Tarjetas
            .Include(t => t.Lista)
            .Include(t => t.AsignadoA)
            .FirstOrDefaultAsync(t => t.Id == id);

        return tarjeta == null ? null : MapToDto(tarjeta);
    }

    public async Task<TarjetaDto> CreateAsync(CreateTarjetaDto dto)
    {
        var maxOrden = await _context.Tarjetas
            .Where(t => t.ListaId == dto.ListaId)
            .MaxAsync(t => (int?)t.Orden) ?? 0;

        var tarjeta = new Tarjeta
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            Estado = "Todo",
            ListaId = dto.ListaId,
            AsignadoAId = dto.AsignadoAId,
            FechaVencimiento = dto.FechaVencimiento,
            Orden = maxOrden + 1
        };

        _context.Tarjetas.Add(tarjeta);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(tarjeta.Id))!;
    }

    public async Task<TarjetaDto?> UpdateAsync(int id, CreateTarjetaDto dto)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(id);
        if (tarjeta == null) return null;

        tarjeta.Titulo = dto.Titulo;
        tarjeta.Descripcion = dto.Descripcion;
        tarjeta.Prioridad = dto.Prioridad;
        tarjeta.FechaVencimiento = dto.FechaVencimiento;
        tarjeta.AsignadoAId = dto.AsignadoAId;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(id);
        if (tarjeta == null) return false;

        _context.Tarjetas.Remove(tarjeta);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MoverTarjetaAsync(int id, int nuevaListaId, int nuevoOrden)
    {
        var tarjeta = await _context.Tarjetas.FindAsync(id);
        if (tarjeta == null) return false;

        tarjeta.ListaId = nuevaListaId;
        tarjeta.Orden = nuevoOrden;

        await _context.SaveChangesAsync();
        return true;
    }

    private static TarjetaDto MapToDto(Tarjeta t)
    {
        return new TarjetaDto
        {
            Id = t.Id,
            Titulo = t.Titulo,
            Descripcion = t.Descripcion,
            Prioridad = t.Prioridad,
            Estado = t.Estado,
            Orden = t.Orden,
            FechaCreacion = t.FechaCreacion,
            FechaVencimiento = t.FechaVencimiento,
            ListaId = t.ListaId,
            NombreLista = t.Lista?.Titulo ?? "",
            AsignadoAId = t.AsignadoAId,
            NombreAsignado = t.AsignadoA?.Nombre
        };
    }
}