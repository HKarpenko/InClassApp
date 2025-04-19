using Application.Interfaces;
using AutoMapper;
using Domain.Models.Dtos;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Services;

public class AccountService(
    IMapper mapper,
    UserManager<AppUser> userManager,
    IStudentService studentService,
    IHttpContextAccessor httpContextAccessor) : IAccountService
{
    public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userModel)
    {
        AppUser user = mapper.Map<AppUser>(userModel);
        var result = await userManager.CreateAsync(user, userModel.Password);

        if (!result.Succeeded)
        {
            return result;
        }

        var newStudent = new Student
        {
            Index = userModel.Index,
            UserId = user.Id
        };

        await studentService.CreateStudent(newStudent);
        await userManager.AddToRoleAsync(user, "Student");

        return result;
    }

    public async Task<bool> LoginUser(UserLoginDto userLoginDto)
    {
        var user = await userManager.FindByEmailAsync(userLoginDto.Email);
        var result = user != null && await userManager.CheckPasswordAsync(user, userLoginDto.Password);
        var userRoles = result ? await userManager.GetRolesAsync(user) : null;

        if (userRoles != null && userRoles.Any())
        {
            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, userRoles.First()));

            await httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        }

        return result;
    }

    public async Task LogoutCurrentUser()
    {
        await httpContextAccessor.HttpContext.SignOutAsync();
    }
}
