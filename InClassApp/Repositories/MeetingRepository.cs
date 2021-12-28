using InClassApp.Data;
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

        public MeetingRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }

        public async new Task<List<Meeting>> GetAll()
        {
            return await _context.Meetings
                .Include(x => x.Group)
                .ToListAsync();
        }

        public async new Task<Meeting> GetById(int id)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();
        }

        public async Task<List<Meeting>> GetMeetingsByGroupId(int groupId)
        {
            return await _context.Meetings
                 .Include(x => x.Group)
                 .Where(x => x.GroupId == groupId)
                 .ToListAsync();
        }
    }
}
