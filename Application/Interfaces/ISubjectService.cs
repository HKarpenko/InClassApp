using Domain.Models.Dtos;

namespace Application.Interfaces
{
    public interface ISubjectService
    {
        Task CreateSubject(SaveSubjectDto subjectDto);
        Task<List<SubjectDto>> GetAllSubjectDtos();
        Task<SubjectDto> GetSubjectDtoById(int id);
        Task UpdateSubject(SaveSubjectDto subject);
        Task DeleteSubject(int id);
    }
}