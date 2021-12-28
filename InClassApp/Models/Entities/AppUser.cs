using Microsoft.AspNetCore.Identity;

namespace InClassApp.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Index { get; set; }
    }
}