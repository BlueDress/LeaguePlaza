using Microsoft.EntityFrameworkCore;

namespace LeaguePlaza.Infrastructure.Data.Repository
{
    public class Repository : IRepository
    {
        private readonly DbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class 
        {
            return await DbSet<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllReadOnlyAsync<T>() where T : class
        {
            return await DbSet<T>().AsNoTracking().ToListAsync();
        }

        private DbSet<T> DbSet<T>() where T : class
        {
            return _context.Set<T>();
        }
    }
}
