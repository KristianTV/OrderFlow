using OrderFlow.Data.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Data.Repository
{
    public abstract class BaseRepository(DbContext _context) : IRepository
    {
        protected DbSet<T> DbSet<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            DbSet<T>().Remove(entity);
        }

        public async Task<bool> ExistsAsync<T>(int id) where T : class
        {
           return await _context.Set<T>().FindAsync(id) == null;
        }

        public async Task<int> SaveChangesAsync()
        {
          return await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await DbSet<T>().AddRangeAsync(entities);
        }

        public IQueryable<T> All<T>() where T : class
        {
           return DbSet<T>().AsQueryable();
        }
    }
}
