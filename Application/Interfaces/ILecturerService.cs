using Domain.Models.Dtos;

namespace Application.Interfaces
{
    public interface ILecturerService
    {
        Task<List<LecturerDto>> GetAllLecturerDtos();
    }
}