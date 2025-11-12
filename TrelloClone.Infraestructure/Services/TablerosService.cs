using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Repositories;

namespace TrelloClone.Infraestructure.Services
{
    public class TablerosService : ITablerosService
    {
        private readonly ITableroRepository _repo;

        public TablerosService(ITableroRepository repo)
        {
            _repo = repo;
        }


        public async Task<List<TableroDto>> GetAllAsync()
        {
            
            var tableros = await _repo.GetAll();
            return tableros.Select(MapToDto).ToList();
        }
        public async Task<TableroDto?> GetByIdAsync(int id)
        {
            var tablero = await _repo.GetByIdWithRelations(id);
            return tablero == null ? null : MapToDto(tablero);
        }

        //Cambio para que el boludo de mateo haga bien las cosas
        public async Task<TableroDto> CreateAsync(CrearTableroDTO dto)
        {
            Console.WriteLine($"UsuarioId antes de guardar: {dto.UsuarioId}");
            var tablero = new Tablero
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Color = dto.Color,
                UsuarioId = dto.UsuarioId
            };

            await _repo.CreateOne(tablero);

            return (await GetByIdAsync(tablero.Id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tablero = await _repo.GetOne(t => t.Id == id);
            if (tablero == null) return false;

            await _repo.DeleteOne(tablero);
            return true;

        }

        //Por ahora no se pueden editar
        public Task<TableroDto?> UpdateAsync(int id, CrearTableroDTO dto)
        {
            throw new NotImplementedException();
        }

        //Metodos extra
        private static ListaConTarjetasDto MapLista(Lista l)
        {
            return new ListaConTarjetasDto
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Orden = l.Orden,
                Tarjetas = l.Tarjetas
                    .OrderBy(tar => tar.Orden)
                    .Select(MapTarjeta)
                    .ToList()
            };
        }
        private static TableroDto MapToDto(Tablero t)
        {
            return new TableroDto
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Color = t.Color,
                FechaCreacion = t.FechaCreacion,
                UsuarioId = t.UsuarioId,
                NombreUsuario = t.Usuario?.Nombre ?? string.Empty,
                Listas = t.Listas?
                    .OrderBy(l => l.Orden)
                    .Select(MapLista)
                    .ToList() ?? new List<ListaConTarjetasDto>()
            };
        }


        private static TarjetaDto MapTarjeta(Tarjeta tar)
        {
            return new TarjetaDto
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
                NombreLista = tar.Lista.Titulo,
                AsignadoAId = tar.AsignadoAId,
                NombreAsignado = tar.AsignadoA?.Nombre
            };
        }

        
    }


}
