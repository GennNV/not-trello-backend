using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Domain.Entities;

namespace TrelloClone.Application.Interfaces
{
    public interface ITablerosService
    {
        Task<List<TableroDto>> GetAllAsync();
        Task<TableroDto?> GetByIdAsync(int id);
        Task<TableroDto> CreateAsync(CrearTableroDTO dto);
        Task<TableroDto?> UpdateAsync(int id, CrearTableroDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<ListaDTO> CreateLista(int tableroId, CreateListaDTO createLista);
    }
}
