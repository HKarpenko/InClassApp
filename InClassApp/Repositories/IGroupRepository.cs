using InClassApp.Models.Entities;
using InClassApp.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IGroupRepository : IBaseRepository<Group> 
    {
        Task<List<Group>> GetGroupsBySubjectId(int subjectId);

        Task<int> AddStudentGroupRelation(int studentId, int groupId);

        Task<bool> DeleteStudentGroupRelation(int studentId, int groupId);
    }
}