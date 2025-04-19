using Application.Interfaces;
using Domain.Models.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace InClassApp.Controllers;

/// <summary>
/// Controller to manage account creation and login processes
/// </summary>
public class AccountController(
    IAccountService accountService) : Controller
{
    /// <summary>
    /// Registration GET endpoint
    /// </summary>
    /// <returns>Registration view</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Registration POST endpoint
    /// </summary>
    /// <param name="userModel">Registration form data</param>
    /// <returns>If user successfully saved, redirect to groups; otherwise shows error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegistrationDto userModel)
    {
        if (!ModelState.IsValid)
        {
            return View(userModel);
        }

        var result = await accountService.RegisterUserAsync(userModel);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return View(userModel);
        }

        return RedirectToAction("Login");
    }

    /// <summary>
    /// Login GET endpoint
    /// </summary>
    /// <returns>Login view</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    /// <summary>
    /// Login POST endpoint
    /// </summary>
    /// <param name="userLoginDto">Registration form data</param>
    /// <returns>If credentials successfully checked, redirect to groups; otherwise shows error message</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(userLoginDto);
        }

        var loginResult = await accountService.LoginUser(userLoginDto);
        if (loginResult)
        {
            return RedirectToAction("Index", "Groups");
        }
        else
        {
            ModelState.AddModelError("", "Invalid UserName or Password");
            return View();
        }
    }

    /// <summary>
    /// Logout POST endpoint
    /// </summary>
    /// <returns>Redirects to Login page</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await accountService.LogoutCurrentUser();
        return RedirectToAction("Login");
    }
}