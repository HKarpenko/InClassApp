using InClassApp.Data;
using InClassApp.Helpers.Interfaces;
using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class MeetingRepository : BaseRepository<Meeting>, IMeetingRepository
    {
        private readonly ApplicationDbContext _context = null;
        private readonly IAttendanceCodeManager _attendanceCodeManager;

        public MeetingRepository(ApplicationDbContext context, IAttendanceCodeManager attendanceCodeManager) : base(context) 
        {
            _context = context;
            _attendanceCodeManager = attendanceCodeManager;
        }

        public async new Task<List<Meeting>> GetAll()
        {
            return await _context.Meetings
                .Include(x => x.Group)
                .Include(x => x.PresenceRecords)
                .ToListAsync();
        }

        public async new Task<Meeting> GetById(int id)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async new Task<Meeting> GetByIdAsNoTracking(int id)
        {
            return await _context.Meetings
                 .AsNoTracking()
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<Meeting>> GetMeetingsByGroupId(int groupId)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Include(x => x.PresenceRecords)
                 .Where(x => x.GroupId == groupId)
                 .ToListAsync();
        }

        public async Task<int> Update(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            await _context.SaveChangesAsync();

            return meeting.Id;
        }
    }
}
