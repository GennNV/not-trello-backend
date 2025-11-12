using Microsoft.EntityFrameworkCore;
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

        public async Task<ListaDTO> CreateLista(int tableroId, CreateListaDTO createLista)
        {

            var lista = await _repo.CreateLista(tableroId, createLista);

            return new ListaDTO
            {
                Titulo = lista.Titulo,
                Orden = lista.Orden,
                Tarjetas = new List<Tarjeta>()
            };
        }

        // Services/TablerosService.cs
        public async Task<TableroDto?> UpdateAsync(int tableroId, List<int> listaIds)
        {
            // Verificar que el tablero existe
            var tablero = await _repo.GetByIdWithRelations(tableroId);
            if (tablero == null)
                throw new KeyNotFoundException($"Tablero con ID:{tableroId} no encontrado");

            // Verificar que todas las listas pertenecen a este tablero
            var listasDelTablero = tablero.Listas.Select(l => l.Id).ToList();
            if (!listaIds.All(id => listasDelTablero.Contains(id)))
                throw new InvalidOperationException("Algunas listas no pertenecen a este tablero");

            // Actualizar el orden de cada lista
            for (int i = 0; i < listaIds.Count; i++)
            {
                var lista = tablero.Listas.FirstOrDefault(l => l.Id == listaIds[i]);
                if (lista != null)
                {
                    lista.Orden = i;
                }
            }

             await _repo.Save();
            return MapToDto(tablero);
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
