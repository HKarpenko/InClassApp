using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IStudentRepository : IBaseRepository<Student> 
    {
        /// <summary>
        /// Gets student by index
        /// </summary>
        /// <param name="index">Students index</param>
        /// <returns>Student by index</returns>
        Task<Student> GetStudentByIndex(string index);

        /// <summary>
        /// Gets student by user id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Student by user id</returns>
        Task<Student> GetStudentByUserId(string userId);

        /// <summary>
        /// Gets students by ids list
        /// </summary>
        /// <param name="ids">Students ids list</param>
        /// <returns>Students list</returns>
        new Task<List<Student>> GetByIds(IEnumerable<int> ids);
    }
}
