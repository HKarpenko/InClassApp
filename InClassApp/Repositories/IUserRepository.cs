using InClassApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Updates the given user
        /// </summary>
        /// <param name="user">User to update</param>
        /// <returns>Result of updating</returns>
        Task<IdentityResult> Update(AppUser user);

        /// <summary>
        /// Gets all the users
        /// </summary>
        /// <returns>Users list</returns>
        Task<List<AppUser>> GetAll();

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User by id</returns>
        Task<AppUser> GetById(string id);
    }
}
