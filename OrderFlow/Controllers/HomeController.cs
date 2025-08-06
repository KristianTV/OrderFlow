using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.ViewModels.System;
using System.Diagnostics;


namespace OrderFlow.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var userId = this.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return View();
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return View();
            }

            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (await userManager.IsInRoleAsync(user, "Speditor"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (await userManager.IsInRoleAsync(user, "Driver"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Driver" });
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                switch (statusCode.Value)
                {
                    case 404:
                        return View("NotFound");
                    case 500:
                        return View("BadRequest");
                }

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
