using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Entities
{
    public class AppUser : IdentityUser
    {
        [ProtectedPersonalData]
        public string? FirstName { get; set; }

        [ProtectedPersonalData]
        public string? LastName { get; set; }
    }
}