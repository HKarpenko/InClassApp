using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class LecturersRepository : BaseRepository<Lecturer>, ILecturersRepository
    {
        private readonly ApplicationDbContext _context = null;

        public LecturersRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Lecturer> GetLecturerByUserId(string userId)
        {
            return _context.Lecturer
                .Include(x => x.Groups)
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefaultAsync();
        }
    }
}
