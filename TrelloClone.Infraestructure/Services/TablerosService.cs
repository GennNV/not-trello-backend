using Microsoft.EntityFrameworkCore;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Application.DTOs.Tarjetas;
using TrelloClone.Application.Interfaces;
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

            return result;
        }
        public Task<TableroDto?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<TableroDto> CreateAsync(CrearTableroDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task<TableroDto?> UpdateAsync(int id, CrearTableroDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
