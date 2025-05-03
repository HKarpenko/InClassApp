using Domain.Models.Dtos;
using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetStudentDtoById(int id);
        Task<StudentDto> GetStudentDtoByIndex(string index);
        Task<StudentDto> GetStudentDtoOfCurrentUser();
        Task<List<StudentDto>> GetStudentDtosByGroupId(int groupId);
        Task<IEnumerable<StudentDto>> GetAllStudents();
        Task<IEnumerable<StudentDto>> GetAllStudentsExcept(List<int> exceptIds);
        Task CreateStudent(Student student);
    }
}
