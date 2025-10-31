using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TrelloClone.Infrastructure.Data;

namespace TrelloClone.Infraestructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        Task<T> GetOne(Expression<Func<T, bool>>? filter = null);
        Task CreateOne(T entity);
        Task UpdateOne(T entity);
        Task DeleteOne(T entity);
        Task Save();
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        async public Task CreateOne(T entity)
        {
            await dbSet.AddAsync(entity);
            await Save();
        }

        async public Task DeleteOne(T entity)
        {
            dbSet.Remove(entity);
            await Save();
        }
        async public Task UpdateOne(T entity)
        {
            dbSet.Update(entity);
            await Save();
        }

        async public Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
            
        }

        async public Task<T> GetOne(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            var result = await query.FirstOrDefaultAsync();

            if (result != null) return result;

            throw new KeyNotFoundException("Entity not found");
        }

        async public Task Save() => await _db.SaveChangesAsync();
    }
}
