using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface ILecturersRepository : IBaseRepository<Lecturer> 
    {
        Task<Lecturer> GetLecturerByUserId(string userId);
    }
}
