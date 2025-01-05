namespace LeaguePlaza.Infrastructure.Data.Repository
{
    public interface IRepository
    {
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;

        Task<IEnumerable<T>> GetAllReadOnlyAsync<T>() where T : class;

        Task<IEnumerable<T>> FindAllReadOnlyAsync<T>(Func<T, bool> predicate) where T : class;
    }
}
