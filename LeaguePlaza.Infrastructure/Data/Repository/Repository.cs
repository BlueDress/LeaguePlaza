using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace LeaguePlaza.Infrastructure.Data.Repository
{
    public class Repository(ApplicationDbContext context) : IRepository
    {
        private readonly DbContext _context = context;

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return await DbSet<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllReadOnlyAsync<T>() where T : class
        {
            return await DbSet<T>().AsNoTracking().ToListAsync();
        }

        public async Task<int> GetCountAsync<T>(Expression<Func<T, bool>> filterCondition) where T : class
        {
            return await DbSet<T>().AsNoTracking().Where(filterCondition).CountAsync();
        }

        public async Task<T?> FindByIdAsync<T>(object id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> filterCondition) where T : class
        {
            return await DbSet<T>().Where(filterCondition).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAllReadOnlyAsync<T>(Expression<Func<T, bool>> filterCondition, params Expression<Func<T, object>>[] includes) where T : class
        {
            IQueryable<T> query = DbSet<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.AsNoTracking().Where(filterCondition).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindSpecificCountReadOnlyAsync<T>(int count, Expression<Func<T, bool>> filterCondition) where T : class
        {
            return await DbSet<T>().AsNoTracking().Where(filterCondition).Take(count).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindSpecificCountOrderedReadOnlyAsync<T, TKey>(int pageNumber, int count, bool orderIsDescending, Expression<Func<T, TKey>> orderCondition, Expression<Func<T, bool>> filterCondition) where T : class
        {
            var query = DbSet<T>().AsNoTracking().Where(filterCondition);
            query = orderIsDescending ? query.OrderByDescending(orderCondition) : query.OrderBy(orderCondition);

            return await query.Skip((pageNumber - 1) * count).Take(count).ToListAsync();
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            DbSet<T>().Update(entity);
        }

        public void Remove<T>(T entity) where T : class
        {
            DbSet<T>().Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private DbSet<T> DbSet<T>() where T : class
        {
            return _context.Set<T>();
        }
    }
}
