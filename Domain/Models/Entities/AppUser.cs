using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }

        public string? Surname { get; set; }
    }
}