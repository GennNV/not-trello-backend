using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.DTOs.Tarjetas;

namespace TrelloClone.Application.Interfaces
{
    internal interface ITablerosService
    {
        Task<List<TableroDto>> GetAllAsync(string? search = null, string? estado = null);
        Task<TableroDto?> GetByIdAsync(int id);
        Task<TableroDto> CreateAsync(CreateTarjetaDto dto);
        Task<TableroDto?> UpdateAsync(int id, CreateTarjetaDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
