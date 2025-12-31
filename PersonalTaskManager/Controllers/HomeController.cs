using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalTaskManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PersonalTaskManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            // Get the logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
        {
            // Get all claims of the user
            var claims = await _userManager.GetClaimsAsync(user);

            // Check if the user has the Admin claim
            bool isAdmin = claims.Any(c => c.Type == "TaskPermission" && c.Value == "Admin");

            if (isAdmin)
            {
                // Admin goes to Admin dashboard
                return RedirectToAction("Dashboard", "Admin");
            }

            // Normal user goes to Task dashboard
            return RedirectToAction("Dashboard", "User");
        }
          
        }

        // Anonymous users see home page
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
