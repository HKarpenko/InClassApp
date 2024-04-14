using Domain.Models.Entities;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public interface IGroupRepository : IBaseRepository<Group> 
    {
        /// <summary>
        /// Gets groups by subject id
        /// </summary>
        /// <param name="subjectId">Subject id</param>
        /// <returns>Groups by subject id</returns>
        Task<List<Group>> GetGroupsBySubjectId(int subjectId);

        /// <summary>
        /// Adds student in group record
        /// </summary>
        /// <param name="studentId">Student id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>New added record id</returns>
        Task<int> AddStudentGroupRelation(int studentId, int groupId);

        /// <summary>
        /// Deletes student in group record
        /// </summary>
        /// <param name="studentId">Student id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>Status of deletion</returns>
        Task<bool> DeleteStudentGroupRelation(int studentId, int groupId);

        /// <summary>
        /// Adds lecturer in group record
        /// </summary>
        /// <param name="lecturerId">Lecturer id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>New added record id</returns>
        Task<int> AddLecturerGroupRelation(int lecturerId, int groupId);

        /// <summary>
        /// Deletes lecturer in group record
        /// </summary>
        /// <param name="lecturerId">Lecturer id</param>
        /// <param name="groupId">Group id</param>
        /// <returns>Status of deletion</returns>
        Task<bool> DeleteLecturerGroupRelation(int lecturerId, int groupId);
    }
}