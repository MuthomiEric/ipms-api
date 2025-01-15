using Core.Entities;

namespace Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<int> CountAsync();
        Task<List<InsurancePolicy>> GetPaginatedAsync(int skip, int take);
    }
}
