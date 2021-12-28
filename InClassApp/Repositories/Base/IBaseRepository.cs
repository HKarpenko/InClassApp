using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories.Base
{
    public interface IBaseRepository<TEntity>
    {
        Task<int> Add(TEntity group);
        Task<int> Update(TEntity group);
        Task<List<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<bool> Delete(int id);
    }
}
