using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;

namespace InClassApp.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context) { }
    }
}
