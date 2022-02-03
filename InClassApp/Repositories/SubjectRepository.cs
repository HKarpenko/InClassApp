using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;

namespace InClassApp.Repositories
{
    /// <summary>
    /// Subjects repository
    /// </summary>
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        /// <summary>
        /// Subjects repository constructor
        /// </summary>
        public SubjectRepository(ApplicationDbContext context) : base(context) { }
    }
}
