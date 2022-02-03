using InClassApp.Data;
using InClassApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    /// <summary>
    /// User repository
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context = null;
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// User repository constructor
        /// </summary>
        public UserRepository(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _context = context;
        }

        /// <summary>
        /// Updates the given user
        /// </summary>
        /// <param name="user">User to update</param>
        /// <returns>Result of updating</returns>
        public async Task<IdentityResult> Update(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return result;
        }

        /// <summary>
        /// Gets all the users
        /// </summary>
        /// <returns>Users list</returns>
        public async Task<List<AppUser>> GetAll()
        {
            return await _userManager.Users
                .ToListAsync();
        }

        /// <summary>
        /// Gets user by id
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>User by id</returns>
        public async Task<AppUser> GetById(string Id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id.Equals(Id));
        }

        /// <summary>
        /// Saves the context changes
        /// </summary>
        public void Save()
        {
            _context.SaveChangesAsync();
        }

    }
}
