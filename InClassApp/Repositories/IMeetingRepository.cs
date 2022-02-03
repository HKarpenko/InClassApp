using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IMeetingRepository : IBaseRepository<Meeting>
    {
        /// <summary>
        /// Gets meetings by group id
        /// </summary>
        /// <param name="groupId">Group id</param>
        /// <returns>Meetings list</returns>
        Task<List<Meeting>> GetMeetingsByGroupId(int groupId);
    }
}
