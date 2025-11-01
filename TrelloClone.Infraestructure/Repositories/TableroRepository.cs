using TrelloClone.Domain.Entities;
using TrelloClone.Infrastructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface ITableroRepository : IRepository<Tablero> { }
    public class TableroRepository : Repository<Tablero>, ITableroRepository
    {
        public TableroRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}
