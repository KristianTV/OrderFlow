namespace OrderFlow.Data.Repository.Contracts
{
    public interface IRepository
    {
        Task AddAsync<T>(T entity) where T : class;
        Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> ExistsAsync<T>(int id) where T : class;
        Task<int> SaveChangesAsync();
    }
}
