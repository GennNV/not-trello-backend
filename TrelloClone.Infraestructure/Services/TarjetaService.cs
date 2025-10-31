using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Repositories;

namespace TrelloClone.Infraestructure.Services;


public class TarjetaService : ITarjetaService
{
    private readonly ITarjetaRepository _repo;

    public TarjetaService(ITarjetaRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<TarjetaDto>> GetAllAsync(string? search = null, string? estado = null)
    {
        var tarjetas = await _repo.GetAll(t =>
            (string.IsNullOrWhiteSpace(search) ||
             t.Titulo.Contains(search) ||
             t.Descripcion.Contains(search)) &&
            (string.IsNullOrWhiteSpace(estado) || t.Estado == estado)
        ); 

        return tarjetas.Select(MapToDto).ToList();
    }

    public async Task<TarjetaDto?> GetByIdAsync(int id)
    {
        var tarjeta = await _repo.GetOne(t => t.Id == id);
        return tarjeta == null ? null : MapToDto(tarjeta);
    }

    public async Task<TarjetaDto> CreateAsync(CreateTarjetaDto dto)
    {

        var maxOrden = await _repo.GetAll(t => t.ListaId == dto.ListaId);
        var tarjeta = new Tarjeta
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            Prioridad = dto.Prioridad,
            Estado = "Todo",
            ListaId = dto.ListaId,
            AsignadoAId = dto.AsignadoAId,
            FechaVencimiento = dto.FechaVencimiento,
            Orden = maxOrden.Count() + 1
        };

        await _repo.CreateOne(tarjeta);
        return (await GetByIdAsync(tarjeta.Id))!;
    }

    public async Task<TarjetaDto?> UpdateAsync(int id, CreateTarjetaDto dto)
    {
        var tarjeta = await _repo.GetOne(t => t.Id == id);
        if (tarjeta == null) return null;

        tarjeta.Titulo = dto.Titulo;
        tarjeta.Descripcion = dto.Descripcion;
        tarjeta.Prioridad = dto.Prioridad;
        tarjeta.FechaVencimiento = dto.FechaVencimiento;
        tarjeta.AsignadoAId = dto.AsignadoAId;

        await _repo.UpdateOne(tarjeta);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tarjeta = await _repo.GetOne(t => t.Id == id);
        if (tarjeta == null) return false;

        await _repo.DeleteOne(tarjeta);
        return true;
    }

    public async Task<bool> MoverTarjetaAsync(int id, MoverTarjetaDTO dto)
    {
        var tarjeta = await _repo.GetOne(t => t.Id == id);
        if (tarjeta == null) return false;

        tarjeta.ListaId = dto.NuevaListaId;
        tarjeta.Orden = dto.NuevoOrden;

        await _repo.UpdateOne(tarjeta);

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