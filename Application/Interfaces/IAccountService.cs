using Domain.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userModel);
    Task<bool> LoginUser(UserLoginDto userLoginDto);
    Task LogoutCurrentUser();
}