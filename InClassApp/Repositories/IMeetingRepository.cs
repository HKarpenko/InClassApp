using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IMeetingRepository : IBaseRepository<Meeting>
    {
        Task<List<Meeting>> GetMeetingsByGroupId(int groupId);
    }
}
