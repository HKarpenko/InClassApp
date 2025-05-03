using Application.Interfaces;
using Domain.Models.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly ILecturersRepository _lecturersRepository;

        public LecturerService(ILecturersRepository lecturersRepository)
        {
            _lecturersRepository = lecturersRepository;
        }

        public async Task<IEnumerable<Lecturer>> GetAllLecturers()
        {
            return await _lecturersRepository.GetAllAsNoTracking().ToListAsync();
        }
    }
}
