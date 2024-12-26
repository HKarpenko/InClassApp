using Domain.Models.Dtos;
using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface ISubjectService
    {
        Task CreateSubject(SaveSubjectDto subjectDto);
        Task<IEnumerable<Subject>> GetAllSubjects();
        Task<Subject> GetSubjectById(int id);
        Task UpdateSubject(SaveSubjectDto subject);
        Task DeleteSubject(int id);
    }
}