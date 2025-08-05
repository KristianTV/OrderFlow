using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.ViewModels.Role;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class RoleController : BaseAdminController
    {
        private readonly ILogger<RoleController> _logger;
        private readonly UserManager<ApplicationUser> userService;

        public RoleController(ILogger<RoleController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            userService = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await userService.Users.ToListAsync();
            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found in the system.");
                return NotFound("No users found.");
            }

            IEnumerable<IndexUserRowsViewModel> usersRows = users.Select(user => new IndexUserRowsViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = userService.GetRolesAsync(user).Result
            }).ToList();

            if (usersRows == null || !usersRows.Any())
            {
                _logger.LogWarning("No users found in the system.");
                return NotFound("No users found.");
            }
            return View(usersRows);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRow(string? id, string? role)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(role))
            {
                return BadRequest("Invalid user ID or role.");
            }

            try
            {
                var user = await userService.FindByIdAsync(id);

                await userService.RemoveFromRolesAsync(user, await userService.GetRolesAsync(user));
                var result = await userService.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index), "Role");
                }
                else
                {
                    _logger.LogError("Failed to change user role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return BadRequest("Failed to change user role.");
                }

            }
            catch
            {
                return BadRequest("Failed to change user role.");
            }
        }
    }

}
