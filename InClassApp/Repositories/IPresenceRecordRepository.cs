using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IPresenceRecordRepository : IBaseRepository<PresenceRecord>
    {
        Task<List<PresenceRecord>> GetPresenceRecordsByMeetingId(int meetingId);
    }
}
