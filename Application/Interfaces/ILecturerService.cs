using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface ILecturerService
    {
        Task<IEnumerable<Lecturer>> GetAllLecturers();
    }
}