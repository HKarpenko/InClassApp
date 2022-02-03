using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IPresenceRecordRepository : IBaseRepository<PresenceRecord>
    {
        /// <summary>
        /// Gets presence records by meeting id
        /// </summary>
        /// <param name="meetingId">Meeting id</param>
        /// <returns>Presence records list</returns>
        Task<List<PresenceRecord>> GetPresenceRecordsByMeetingId(int meetingId);
    }
}
