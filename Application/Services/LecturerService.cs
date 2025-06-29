using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class LecturerService(
    ILecturersRepository lecturersRepository,
    IMapper mapper) : ILecturerService
{
    public async Task<List<LecturerDto>> GetAllLecturerDtos()
    {
        var ls = await lecturersRepository.GetAllAsNoTracking()
            .Include(l => l.User)
            .ToListAsync();
        return mapper.Map<List<LecturerDto>>(ls);
    }
}
