using Domain.Models;
using Domain.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InClassApp.Controllers;

/// <summary>
/// Controller for home view
/// </summary>
public class HomeController(
    UserManager<AppUser> userManager) : Controller
{
    /// <summary>
    /// Gets home view
    /// </summary>
    /// <returns>Home view</returns>
    public async Task<IActionResult> Index()
    {
        var currentUser = await userManager.GetUserAsync(HttpContext.User);
        if (currentUser != null)
        {
            return RedirectToAction("Index", "Groups");
        }

        return View();
    }

    /// <summary>
    /// Gets error view
    /// </summary>
    /// <returns>Error view</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}