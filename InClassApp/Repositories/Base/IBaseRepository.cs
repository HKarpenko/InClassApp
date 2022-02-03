using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories.Base
{
    public interface IBaseRepository<TEntity>
    {
        /// <summary>
        /// Adds entity to db
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The added enity id</returns>
        Task<int> Add(TEntity entity);

        /// <summary>
        /// Updates the given entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <returns>The updated entity id</returns>
        Task<int> Update(TEntity entity);

        /// <summary>
        /// Gets all the entities
        /// </summary>
        /// <returns>Entities list</returns>
        Task<List<TEntity>> GetAll();

        /// <summary>
        /// Gets entity by id
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity by id</returns>
        Task<TEntity> GetById(int id);

        /// <summary>
        /// Gets entity by id as no tracking
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Entity by id</returns>
        Task<TEntity> GetByIdAsNoTracking(int id);

        /// <summary>
        /// Gets entities by ids list
        /// </summary>
        /// <param name="ids">Entities ids list</param>
        /// <returns>Entities list</returns>
        Task<List<TEntity>> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Deletes entity from db
        /// </summary>
        /// <param name="id">Entity id</param>
        /// <returns>Result of deletion</returns>
        Task<bool> Delete(int id);

        /// <summary>
        /// Saves the context
        /// </summary>
        void Save();
    }
}
