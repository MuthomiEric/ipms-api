using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly InsuranceContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(InsuranceContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<int> CountAsync()
        {
            return await _context.InsurancePolicies.CountAsync();
        }

        public async Task<List<InsurancePolicy>> GetPaginatedAsync(int skip, int take)
        {
            return await _context.InsurancePolicies
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }

}
