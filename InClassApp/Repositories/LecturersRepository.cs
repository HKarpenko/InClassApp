using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public new Task<List<Lecturer>> GetAll()
        {
            return _context.Lecturer
                .Include(x => x.User)
                .Include(x => x.LecturerGroupRelations)
                .ToListAsync();
        }

        public Task<Lecturer> GetLecturerByUserId(string userId)
        {
            return _context.Lecturer
                .Include(x => x.User)
                .Include(x => x.LecturerGroupRelations)
                .Where(x => x.UserId.Equals(userId))
                .FirstOrDefaultAsync();
        }
    }
}
