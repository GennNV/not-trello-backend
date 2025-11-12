using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface ITableroRepository : IRepository<Tablero>
    {
        Task<ListaDTO> CreateLista(int tableroId, CreateListaDTO createLista);
        Task<Tablero?> GetByIdWithRelations(int id);
        Task<Lista?> GetOneListById(int listId);
        Task<bool> DeleteLista(Lista lista);
       
    }

    public class TableroRepository : Repository<Tablero>, ITableroRepository
    {
        private readonly ApplicationDbContext _context;

        public TableroRepository(ApplicationDbContext db) : base(db)
        {
            _context = db;
        }
        public async Task<ListaDTO> CreateLista(int tableroId, CreateListaDTO createLista)
        {
            var lista = new Lista
            {
                Titulo = createLista.Titulo,
                Orden = createLista.Orden,
                TableroId = tableroId,
                Tarjetas = new List<Tarjeta>()
            };
            _context.Listas.Add(lista);
            await _context.SaveChangesAsync();
            return new ListaDTO
            {
                Titulo = lista.Titulo,
                Orden = lista.Orden,
                Tarjetas = lista.Tarjetas
            };
        }

        public async Task<Tablero?> GetByIdWithRelations(int id)
        {
            return await _context.Tableros
                .Include(t => t.Usuario)
                .Include(t => t.Listas)
                    .ThenInclude(l => l.Tarjetas)
                        .ThenInclude(tar => tar.AsignadoA)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Lista?> GetOneListById(int listId)
        {
            return await _context.Listas
                .Include(l => l.Tarjetas)
                .FirstOrDefaultAsync(l => l.Id == listId);
        }

        public async Task<bool> DeleteLista(Lista lista)
        {
            _context.Listas.Remove(lista);
            await _context.SaveChangesAsync();
            return true;
        }
        public override async Task<IEnumerable<Tablero>> GetAll(Expression<Func<Tablero, bool>>? filter = null)
        {
            IQueryable<Tablero> query = _context.Tableros
                .Include(t => t.Usuario)
                .Include(t => t.Listas)
                    .ThenInclude(l => l.Tarjetas)
                        .ThenInclude(tar => tar.AsignadoA);

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }


    }
}
