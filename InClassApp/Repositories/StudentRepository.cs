using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    /// <summary>
    /// Students repository
    /// </summary>
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        private readonly ApplicationDbContext _context = null;

        /// <summary>
        /// Students repository constructor
        /// </summary>
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the students
        /// </summary>
        /// <returns>Students list</returns>
        public async new Task<List<Student>> GetAll()
        {
            return await _context.Student
                .Include(x => x.User)
                .ToListAsync();
        }

        /// <summary>
        /// Gets student by id
        /// </summary>
        /// <param name="id">Student id</param>
        /// <returns>Student by id</returns>
        public async new Task<Student> GetById(int id)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets students by ids list
        /// </summary>
        /// <param name="ids">Students ids list</param>
        /// <returns>Students list</returns>
        public async new Task<List<Student>> GetByIds(IEnumerable<int> ids)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => ids.Contains(x.Id))
                 .ToListAsync();
        }

        /// <summary>
        /// Gets student by index
        /// </summary>
        /// <param name="index">Students index</param>
        /// <returns>Student by index</returns>
        public async Task<Student> GetStudentByIndex(string index)
        {
            return await _context.Student
                 .Include(x => x.User)
                 .Where(x => x.Index == index)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets student by user id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Student by user id</returns>
        public async Task<Student> GetStudentByUserId(string userId)
        {
            return await _context.Student
                 .Include(x => x.StudentGroupRelations)
                 .Where(x => x.UserId == userId)
                 .FirstOrDefaultAsync();
        }
    }
}
