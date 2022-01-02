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
                .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .ToListAsync();
        }

        public async new Task<Group> GetById(int id)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.Meetings)
                 .Include(x => x.Lecturer)
                    .ThenInclude(l => l.User)
                 .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<Group>> GetGroupsBySubjectId(int subjectId)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.Lecturer)
                    .ThenInclude(l => l.User)
                 .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                 .Where(x => x.SubjectId == subjectId)
                 .ToListAsync();
        }

        public async Task<int> AddStudentGroupRelation(int studentId, int groupId)
        {
            var relation = new StudentGroupRelation
            {
                StudentId = studentId,
                GroupId = groupId
            };
            _context.Set<StudentGroupRelation>().Add(relation);
            await _context.SaveChangesAsync();
            
            return relation.Id;
        }

        public async Task<bool> DeleteStudentGroupRelation(int studentId, int groupId)
        {
            var group = await GetById(groupId);
            var relation = group.StudentGroupRelations.FirstOrDefault(r => r.StudentId == studentId);
            if(relation == null)
            {
                return false;
            }

            _context.Set<StudentGroupRelation>().Remove(relation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
