using Infrastructure.Data;
using Domain.Models.Entities;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Group repository
    /// </summary>
    public class GroupRepository : BaseRepository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _context = null;

        /// <summary>
        /// Group repository constructor
        /// </summary>
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the groups
        /// </summary>
        /// <returns>All groups</returns>
        public async new Task<List<Group>> GetAll()
        {
            return await _context.Groups
                .Include(x => x.Subject)
                .Include(x => x.LecturerGroupRelations)
                    .ThenInclude(r => r.Lecturer)
                        .ThenInclude(l => l.User)
                .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                .ToListAsync();
        }

        /// <summary>
        /// Gets group by id
        /// </summary>
        /// <param name="id">Group id</param>
        /// <returns>Group</returns>
        public async new Task<Group> GetById(int id)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.Meetings)
                 .Include(x => x.LecturerGroupRelations)
                    .ThenInclude(r => r.Lecturer)
                        .ThenInclude(l => l.User)
                 .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets groups by subject id
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <returns>Groups by subject id</returns>
        public async Task<List<Group>> GetGroupsBySubjectId(int subjectId)
        {
            return await _context.Groups
                 .Include(x => x.Subject)
                 .Include(x => x.LecturerGroupRelations)
                    .ThenInclude(r => r.Lecturer)
                        .ThenInclude(l => l.User)
                 .Include(x => x.StudentGroupRelations)
                    .ThenInclude(r => r.Student)
                        .ThenInclude(s => s.User)
                 .Where(x => x.SubjectId == subjectId)
                 .ToListAsync();
        }

        /// <summary>
        /// Adds student in group record
        /// </summary>
        /// <param name="studentId">Student id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>New added record id</returns>
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

        /// <summary>
        /// Deletes student in group record
        /// </summary>
        /// <param name="studentId">Student id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>Status of deletion</returns>
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

        /// <summary>
        /// Adds lecturer in group record
        /// </summary>
        /// <param name="lecturerId">Lecturer id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>New added record id</returns>
        public async Task<int> AddLecturerGroupRelation(int lecturerId, int groupId)
        {
            var currentRelation = (await GetById(groupId)).LecturerGroupRelations.FirstOrDefault(x => x.LecturerId == lecturerId);
            if (currentRelation != null)
            {
                return currentRelation.Id;
            }

            var relation = new LecturerGroupRelation
            {
                LecturerId = lecturerId,
                GroupId = groupId
            };
            _context.Set<LecturerGroupRelation>().Add(relation);
            await _context.SaveChangesAsync();

            return relation.Id;
        }

        /// <summary>
        /// Deletes lecturer in group record
        /// </summary>
        /// <param name="lecturerId">Lecturer id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>Status of deletion</returns>
        public async Task<bool> DeleteLecturerGroupRelation(int lecturerId, int groupId)
        {
            var group = await GetById(groupId);
            var relation = group.LecturerGroupRelations.FirstOrDefault(r => r.LecturerId == lecturerId);
            if (relation == null)
            {
                return false;
            }

            _context.Set<LecturerGroupRelation>().Remove(relation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
