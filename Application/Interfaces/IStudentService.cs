using Domain.Models.Dtos;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetStudentDtoById(int id);
        Task<StudentDto> GetStudentDtoByIndex(string index);
        Task<List<StudentDto>> GetStudentDtosByGroupId(int groupId);
        Task<IEnumerable<StudentDto>> GetAllStudents();
        Task<IEnumerable<StudentDto>> GetAllStudentsExcept(List<int> exceptIds);
    }
}
