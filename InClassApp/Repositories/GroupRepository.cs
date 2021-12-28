using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _context = null;

        public GroupRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async new Task<List<Group>> GetAll()
        {
            return await _context.Groups
                .Include(x => x.Subject)
                .Include(x => x.Lecturer)
                    .ThenInclude(l => l.User)
                .ToListAsync();
        }

        public async new Task<Group> GetById(int id)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.Lecturer)
                    .ThenInclude(l => l.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<Group>> GetGroupsBySubjectId(int subjectId)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.Lecturer)
                    .ThenInclude(l => l.User)
                 .Where(x => x.SubjectId == subjectId)
                 .ToListAsync();
        }
    }
}
