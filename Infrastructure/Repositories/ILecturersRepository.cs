using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public interface ILecturersRepository : IBaseRepository<Lecturer> 
    {
        /// <summary>
        /// Gets lecturer by user id as no tracking
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Lecturer by user id</returns>
        Task<Lecturer?> GetLecturerByUserIdAsNoTracking(string userId);
    }
}
