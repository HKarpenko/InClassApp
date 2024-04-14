using Infrastructure.Data;
using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
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
