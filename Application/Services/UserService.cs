using Application.Interfaces;
using Domain.Models.Entities;
using Domain.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(IServiceProvider serviceProvider)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        }

        public async Task<UserRole?> GetUserMainRole(AppUser currentUser)
        {
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            if (currentUserRoles == null)
            {
                return null;
            }
            else if (currentUserRoles.Contains(nameof(UserRole.Admin)))
            {
                return UserRole.Admin;
            }
            else if (currentUserRoles.Contains(nameof(UserRole.Lecturer)))
            {
                return UserRole.Lecturer;
            }
            else if (currentUserRoles.Contains(nameof(UserRole.Student)))
            {
                return UserRole.Student;
            }

            throw new ArgumentException($"Unknown roles provided: {currentUserRoles}");
        }
    }
}
