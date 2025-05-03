using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Interfaces
{
    public interface IPresenceRecordRepository : IBaseRepository<PresenceRecord>
    {
        /// <summary>
        /// Gets presence records by meeting id
        /// </summary>
        /// <param name="meetingId">Meeting id</param>
        /// <returns>Presence records list</returns>
        IQueryable<PresenceRecord> GetPresenceRecordsByMeetingId(int meetingId);
    }
}
