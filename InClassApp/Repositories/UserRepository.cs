using InClassApp.Data;
using InClassApp.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context = null;
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _context = context;
        }

        public async Task<IdentityResult> Update(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return result;
        }
        public async Task<List<AppUser>> GetAll()
        {
            return await _userManager.Users
                .ToListAsync();
        }

        public async Task<AppUser> GetById(string Id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id.Equals(Id));
        }

        public void Save()
        {
            _context.SaveChangesAsync();
        }

    }
}
