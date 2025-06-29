using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace LeaguePlaza.Infrastructure.Data.Repository
{
    public interface IRepository
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;

        Task<IEnumerable<T>> GetAllReadOnlyAsync<T>() where T : class;

        Task<int> GetCountAsync<T>(Expression<Func<T, bool>> filterCondition) where T : class;

        Task<T?> FindByIdAsync<T>(object id) where T : class;

        Task<IEnumerable<T>> FindAllAsync<T>(Expression<Func<T, bool>> filterCondition) where T : class;

        Task<T?> FindOneAsync<T>(Expression<Func<T, bool>> filterCondition, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : class;

        Task<T?> FindOneReadOnlyAsync<T>(Expression<Func<T, bool>> filterCondition, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : class;

        Task<IEnumerable<T>> FindAllReadOnlyAsync<T>(Expression<Func<T, bool>> filterCondition, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : class;

        Task<IEnumerable<T>> FindSpecificCountReadOnlyAsync<T>(int count, Expression<Func<T, bool>> filterCondition) where T : class;

        Task<IEnumerable<T>> FindSpecificCountOrderedReadOnlyAsync<T, TKey>(int pageNumber, int count, bool orderIsDescending, Expression<Func<T, TKey>> orderCondition, Expression<Func<T, bool>> filterCondition, params Func<IQueryable<T>, IIncludableQueryable<T, object>>[] includes) where T : class;

        Task AddAsync<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        void Remove<T>(T entity) where T : class;

        Task<int> SaveChangesAsync();
    }
}
