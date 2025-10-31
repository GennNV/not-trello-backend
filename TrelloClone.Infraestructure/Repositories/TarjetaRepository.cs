
using TrelloClone.Domain.Entities;
using TrelloClone.Infrastructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface ITarjetaRepository : IRepository<Tarjeta>{}
    public class TarjetaRepository : Repository<Tarjeta>, ITarjetaRepository
    {
        public TarjetaRepository(ApplicationDbContext db) : base(db)
        {

        }
    }
}
