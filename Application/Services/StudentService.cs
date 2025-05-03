using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StudentService(
    IMapper mapper, 
    IStudentRepository studentRepository,
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IStudentService
{
    public async Task<StudentDto> GetStudentDtoById(int id)
    {
        return mapper.Map<StudentDto>(await studentRepository.GetByIdAsNoTracking(id));
    }

    public async Task<StudentDto> GetStudentDtoByIndex(string index)
    {
        return mapper.Map<StudentDto>(await studentRepository.GetStudentByIndex(index));
    }

    public async Task<StudentDto> GetStudentDtoOfCurrentUser()
    {
        var userId = (await userManager.GetUserAsync(httpContextAccessor.HttpContext.User))?.Id;
        if (userId == null)
        {
            throw new NullReferenceException("Current user doesn't exist");
        }
        var student = await studentRepository.GetStudentByUserIdAsNoTracking(userId) ?? throw new NullReferenceException("Current user doesn't have student account");
        return mapper.Map<StudentDto>(student);
    }

    public async Task<List<StudentDto>> GetStudentDtosByGroupId(int groupId)
    {
        var groupStudents = await studentRepository.GetAllAsNoTracking()
            .Where(s => s.StudentGroupRelations.Any(s => s.GroupId == groupId))
            .ToListAsync();
        return mapper.Map<List<StudentDto>>(groupStudents);
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudents()
    {
        var students = await studentRepository.GetAllAsNoTracking().ToListAsync();
        return mapper.Map<List<StudentDto>>(students);
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudentsExcept(List<int> exceptIds)
    {
        var students = await studentRepository.GetAllAsNoTracking()
            .Where(s => !exceptIds.Contains(s.Id))
            .ToListAsync();
        return mapper.Map<List<StudentDto>>(students);
    }

    public async Task CreateStudent(Student student)
    {
        await studentRepository.Add(student);
    }
}