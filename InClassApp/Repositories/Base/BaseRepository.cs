using InClassApp.Data;
using InClassApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
    {
        private readonly ApplicationDbContext _context = null;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;

        }

        public async Task<int> Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>()
                .ToListAsync();
        }

        public async Task<TEntity> GetById(int id)
        {
            return await _context.Set<TEntity>()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetByIdAsNoTracking(int id)
        {
            return await _context.Set<TEntity>()
                 .AsNoTracking()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<TEntity>> GetByIds(IEnumerable<int> ids)
        {
            return await _context.Set<TEntity>()
                 .Where(x => ids.Contains(x.Id))
                 .ToListAsync();
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public void Save()
        {
            _context.SaveChangesAsync();
        }
    }
}
