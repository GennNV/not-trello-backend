using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TrelloClone.Domain.Entities;
using TrelloClone.Infraestructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
    }
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private readonly ApplicationDbContext _db;

        public UsuarioRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        async new public Task<Usuario> GetOne(Expression<Func<Usuario, bool>>? filter = null)
        {
            IQueryable<Usuario> query = dbSet;
            return await query.FirstOrDefaultAsync();
        }
    }
}
