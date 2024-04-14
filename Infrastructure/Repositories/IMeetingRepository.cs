using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
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
