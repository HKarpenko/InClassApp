using InClassApp.Models.Dtos;
using InClassApp.Models.Entities;
using InClassApp.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InClassApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStudentRepository _studentRepository;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(IServiceProvider serviceProvider, IStudentRepository studentRepository, SignInManager<AppUser> signInManager)
        {
            _userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            _studentRepository = studentRepository;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistrationDto userModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userModel);
            }

            AppUser user = new AppUser
            {
                Name = userModel.FirstName,
                Surname = userModel.LastName,
                Email = userModel.Email,
                UserName = userModel.FirstName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return View(userModel);
            }

            var newStudent = new Student
            {
                Index = userModel.Index,
                UserId = user.Id
            };

            await _studentRepository.Add(newStudent);
            await _userManager.AddToRoleAsync(user, "Student");
            return RedirectToAction("Index", "Groups");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginDto);
            }

            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user != null &&
                await _userManager.CheckPasswordAsync(user, userLoginDto.Password))
            {
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, (await _userManager.GetRolesAsync(user)).FirstOrDefault()));
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(identity));
                return RedirectToAction("Index", "Groups");
            }
            else
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
