using Domain.Models.Entities;
using Domain.Models.Enums;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<UserRole?> GetUserMainRole(AppUser currentUser);
    }
}