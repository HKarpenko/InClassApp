using InClassApp.Data;
using InClassApp.Helpers.Interfaces;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    /// <summary>
    /// Meetings repository
    /// </summary>
    public class MeetingRepository : BaseRepository<Meeting>, IMeetingRepository
    {
        private readonly ApplicationDbContext _context = null;
        private readonly IAttendanceCodeManager _attendanceCodeManager;

        /// <summary>
        /// Meetings repository constructor
        /// </summary>
        public MeetingRepository(ApplicationDbContext context, IAttendanceCodeManager attendanceCodeManager) : base(context) 
        {
            _context = context;
            _attendanceCodeManager = attendanceCodeManager;
        }

        /// <summary>
        /// Gets all the meetings
        /// </summary>
        /// <returns>Meetings list</returns>
        public async new Task<List<Meeting>> GetAll()
        {
            return await _context.Meetings
                .Include(x => x.Group)
                .Include(x => x.PresenceRecords)
                .ToListAsync();
        }

        /// <summary>
        /// Gets meeting by id
        /// </summary>
        /// <param name="id">Meeting id</param>
        /// <returns>Meeting by id</returns>
        public async new Task<Meeting> GetById(int id)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets meeting by id as no tracking
        /// </summary>
        /// <param name="id">Meeting id</param>
        /// <returns>Meeting by id</returns>
        public async new Task<Meeting> GetByIdAsNoTracking(int id)
        {
            return await _context.Meetings
                 .AsNoTracking()
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets meetings by group id
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <returns>Meetings list</returns>
        public async Task<List<Meeting>> GetMeetingsByGroupId(int groupId)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.GroupId == groupId)
                 .ToListAsync();
        }

        /// <summary>
        /// Updates the meeting
        /// </summary>
        /// <param name="meeting">Meeting to update</param>
        /// <returns>Id of updated meeting</returns>
        public async Task<int> Update(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();

            return meeting.Id;
        }
    }
}
