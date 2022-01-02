using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class PresenceRecordRepository : BaseRepository<PresenceRecord>, IPresenceRecordRepository
    {
        private readonly ApplicationDbContext _context = null;

        public PresenceRecordRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
