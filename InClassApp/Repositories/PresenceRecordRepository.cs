using InClassApp.Data;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
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
        public async new Task<List<PresenceRecord>> GetAll()
        {
            return await _context.PresenceRecords
                .Include(x => x.Meeting)
                    .ThenInclude(x => x.Group)
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .ToListAsync();
        }

        public async new Task<PresenceRecord> GetById(int id)
        {
            return await _context.PresenceRecords
                 .Include(x => x.Meeting)
                    .ThenInclude(x => x.Group)
                 .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }


        public async Task<List<PresenceRecord>> GetPresenceRecordsByMeetingId(int meetingId)
        {
            return await _context.PresenceRecords
                .Include(x => x.Meeting)
                    .ThenInclude(x => x.Group)
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .Where(x => x.MeetingId == meetingId)
                .ToListAsync();
        }
    }
}
