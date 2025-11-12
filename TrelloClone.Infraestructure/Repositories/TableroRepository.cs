using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrelloClone.Application.DTOs.Tableros;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface ITableroRepository : IRepository<Tablero>
    {
        public Task<ListaDTO> CreateLista(int tableroId, CreateListaDTO createLista);
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
