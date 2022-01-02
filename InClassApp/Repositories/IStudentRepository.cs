using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IStudentRepository : IBaseRepository<Student> 
    {
        Task<Student> GetStudentByIndex(string index);

        new Task<List<Student>> GetByIds(IEnumerable<int> ids);
    }
}
