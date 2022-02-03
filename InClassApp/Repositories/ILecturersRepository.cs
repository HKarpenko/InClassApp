using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface ILecturersRepository : IBaseRepository<Lecturer> 
    {
        /// <summary>
        /// Gets lecturer by user id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Lecturer by user id</returns>
        Task<Lecturer> GetLecturerByUserId(string userId);
    }
}
