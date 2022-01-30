using InClassApp.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InClassApp.Repositories
{
    public interface IUserRepository
    {
        Task<IdentityResult> Update(AppUser user);
        Task<List<AppUser>> GetAll();
        Task<AppUser> GetById(string id);
    }
}
