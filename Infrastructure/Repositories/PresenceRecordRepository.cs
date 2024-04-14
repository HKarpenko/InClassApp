using Infrastructure.Data;
using Domain.Models.Entities;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    /// <summary>
    /// Presence records repository
    /// </summary>
    public class PresenceRecordRepository : BaseRepository<PresenceRecord>, IPresenceRecordRepository
    {
        private readonly ApplicationDbContext _context = null;

        /// <summary>
        /// Presence records repository constructor
        /// </summary>
        /// <param name="context"></param>
        public PresenceRecordRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the presence records
        /// </summary>
        /// <returns>Presence records list</returns>
        public async new Task<List<PresenceRecord>> GetAll()
        {
            return await _context.PresenceRecords
                .Include(x => x.Meeting)
                    .ThenInclude(x => x.Group)
                .Include(x => x.Student)
                    .ThenInclude(x => x.User)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the presence record by id
        /// </summary>
        /// <param name="id">Presence record by id</param>
        /// <returns>Presence record</returns>
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

        /// <summary>
        /// Gets presence records by meeting id
        /// </summary>
        /// <param name="meetingId">Meeting id</param>
        /// <returns>Presence records list</returns>
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
