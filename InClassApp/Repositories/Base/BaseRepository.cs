using InClassApp.Data;
using InClassApp.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories.Base
{
    /// <summary>
    /// Base repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type to manage with</typeparam>
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : Entity
    {
        private readonly ApplicationDbContext _context = null;

        /// <summary>
        /// Base repository constructor
        /// </summary>
        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds entity to db
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The added enity id</returns>
        public async Task<int> Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;

        }

        /// <summary>
        /// Updates the given entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The updated entity id</returns>
        public async Task<int> Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        /// <summary>
        /// Gets all the entities
        /// </summary>
        /// <returns>Entities list</returns>
        public async Task<List<TEntity>> GetAll()
        {
            return await _context.Set<TEntity>()
                .ToListAsync();
        }

        /// <summary>
        /// Gets entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity by id</returns>
        public async Task<TEntity> GetById(int id)
        {
            return await _context.Set<TEntity>()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets entity by id as no tracking
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity by id</returns>
        public async Task<TEntity> GetByIdAsNoTracking(int id)
        {
            return await _context.Set<TEntity>()
                 .AsNoTracking()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets entities by ids list
        /// </summary>
        /// <param name="ids">Entities ids list</param>
        /// <returns>Entities list</returns>
        public async Task<List<TEntity>> GetByIds(IEnumerable<int> ids)
        {
            return await _context.Set<TEntity>()
                 .Where(x => ids.Contains(x.Id))
                 .ToListAsync();
        }

        /// <summary>
        /// Deletes entity from db
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Result of deletion</returns>
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Saves the context
        /// </summary>
        public void Save()
        {
            _context.SaveChangesAsync();
        }
    }
}
