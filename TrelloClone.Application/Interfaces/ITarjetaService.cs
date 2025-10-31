using TrelloClone.Application.DTOs.Tarjetas;

namespace TrelloClone.Application.Interfaces;

public interface ITarjetaService
{
    Task<List<TarjetaDto>> GetAllAsync(string? search = null, string? estado = null);
    Task<TarjetaDto?> GetByIdAsync(int id);
    Task<TarjetaDto> CreateAsync(CreateTarjetaDto dto);
    Task<TarjetaDto?> UpdateAsync(int id, CreateTarjetaDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> MoverTarjetaAsync(int id, MoverTarjetaDTO dto);
}
