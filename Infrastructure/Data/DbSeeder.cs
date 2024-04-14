using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Data
{
    public class DbSeeder : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public DbSeeder(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                await dbContext.SaveChangesAsync();

                var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                string[] roleNames = { "Admin", "Lecturer", "Student" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
                var x = _configuration["AdminUserSettings:UserEmail"];
                AppUser _user = await UserManager.FindByEmailAsync(x);
                if (_user == null)
                {
                    //Here you could create a super user who will maintain the web app
                    var powerUser = new AppUser
                    {
                        UserName = _configuration["AdminUserSettings:UserName"],
                        Email = _configuration["AdminUserSettings:UserEmail"],
                    };
                    //Ensure you have these values in your appsettings.json file
                    string powerUserPassword = _configuration["AdminUserSettings:UserPassword"];

                    var createPowerUser = await UserManager.CreateAsync(powerUser, powerUserPassword);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the role
                        await UserManager.AddToRoleAsync(powerUser, "Admin");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
