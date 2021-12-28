using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;

namespace InClassApp.Repositories
{
    public class LecturersRepository : BaseRepository<Lecturer>, ILecturersRepository
    {
        public LecturersRepository(ApplicationDbContext context) : base(context) { }
    }
}
