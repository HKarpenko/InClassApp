using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
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
