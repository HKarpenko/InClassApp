using InClassApp.Models;
using InClassApp.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InClassApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<PresenceRecord> PresenceRecords { get; set; }
        public DbSet<InClassApp.Models.Entities.Student> Student { get; set; }
    }
}
