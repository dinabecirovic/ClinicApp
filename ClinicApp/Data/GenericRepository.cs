using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicApp.Data
{
    public class GenericRepository<T> where T : class
    {
        private readonly ClinicDbContext _ctx;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ClinicDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Set<T>();
        }

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _ctx.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _ctx.SaveChangesAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
