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

        public async Task<T> FindByIdAsync<T>(object id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> FindAllReadOnlyAsync<T>(Func<T, bool> predicate) where T : class
        {
            return await Task.Run(() => DbSet<T>().AsNoTracking().Where(predicate));
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
