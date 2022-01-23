using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext _context = null;

        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async new Task<List<Student>> GetAll()
        {
            return await _context.Student
                .Include(x => x.User)
                .ToListAsync();
        }

        public async new Task<Student> GetById(int id)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async new Task<List<Student>> GetByIds(IEnumerable<int> ids)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => ids.Contains(x.Id))
                 .ToListAsync();
        }

        public async Task<Student> GetStudentByIndex(string index)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => x.Index == index)
                 .FirstOrDefaultAsync();
        }

        public async Task<Student> GetStudentByUserId(string userId)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Include(x => x.StudentGroupRelations)
                 .Where(x => x.UserId == userId)
                 .FirstOrDefaultAsync();
        }
    }
}
