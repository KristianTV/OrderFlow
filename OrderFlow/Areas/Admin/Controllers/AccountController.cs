using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.Role;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : BaseAdminController
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService accountService;
        private readonly IRealtimeNotifier realtimeNotifier;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService,
            IRealtimeNotifier? realtimeNotifier = null)
        {
            _logger = logger;
            _userManager = userManager;
            this.accountService = accountService;
            this.realtimeNotifier = realtimeNotifier ?? NullRealtimeNotifier.Instance;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? sortOrder = null)
        {
            var users = await _userManager.Users
                .Include(user => user.PersonalProfile)
                .Include(user => user.CompanyProfile)
                .ToListAsync();

            if (users == null || !users.Any())
            {
                _logger.LogWarning("No users found in the system.");
                return NotFound();
            }

            List<IndexUserRowsViewModel> usersRows = new List<IndexUserRowsViewModel>();

            foreach (ApplicationUser user in users)
            {
                usersRows.Add(new IndexUserRowsViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    AccountType = user.AccountType.ToString(),
                    DisplayName = user.PersonalProfile != null
                        ? $"{user.PersonalProfile.FirstName} {user.PersonalProfile.LastName}".Trim()
                        : user.CompanyProfile?.CompanyName ?? string.Empty,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            if (usersRows == null || !usersRows.Any())
            {
                _logger.LogWarning("No users found in the system.");
                return NotFound();
            }

            string currentSortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "username_asc" : sortOrder;
            usersRows = SortUsers(usersRows, currentSortOrder).ToList();
            ViewData["CurrentSortOrder"] = currentSortOrder;

            return View(usersRows);
        }

        private static IEnumerable<IndexUserRowsViewModel> SortUsers(IEnumerable<IndexUserRowsViewModel> users, string sortOrder)
        {
            return sortOrder switch
            {
                "username_desc" => users.OrderByDescending(user => user.UserName),
                "email_asc" => users.OrderBy(user => user.Email),
                "email_desc" => users.OrderByDescending(user => user.Email),
                "display_name_asc" => users.OrderBy(user => user.DisplayName),
                "display_name_desc" => users.OrderByDescending(user => user.DisplayName),
                "account_type_asc" => users.OrderBy(user => user.AccountType),
                "account_type_desc" => users.OrderByDescending(user => user.AccountType),
                "role_asc" => users.OrderBy(user => user.Roles.FirstOrDefault() ?? string.Empty),
                "role_desc" => users.OrderByDescending(user => user.Roles.FirstOrDefault() ?? string.Empty),
                _ => users.OrderBy(user => user.UserName)
            };
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string? id)
        {
            ApplicationUser? user = await accountService.GetCurrentUserWithProfilesAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            ProfileViewModel model = user.MapProfileViewModel();
            ViewBag.UserId = user.Id;
            ViewBag.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            ApplicationUser? user = await accountService.GetCurrentUserWithProfilesAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            ViewBag.UserId = user.Id;

            return View(user.MapEditProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, EditProfileViewModel model)
        {
            ApplicationUser? user = await accountService.GetCurrentUserWithProfilesAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", id);
                return NotFound();
            }

            model.AccountType = user.AccountType;
            ViewBag.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult result = await accountService.UpdateUserAndProfileAsync(id, model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User data updated successfully.";
                await NotifyAccountChangedAsync("Updated", user.Id);
                return RedirectToAction(nameof(Detail), new { id });
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRow(string? id, string? role)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(role))
            {
                _logger.LogWarning("Invalid user ID or role.");
                ModelState.AddModelError(string.Empty, "Invalid user ID or role.");
                return BadRequest();
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", id);
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return NotFound();
                }

                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    await NotifyAccountChangedAsync("RoleChanged", user.Id);
                    return RedirectToAction(nameof(Index), "Account");
                }
                else
                {
                    _logger.LogError("Failed to change user role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    ModelState.AddModelError(string.Empty, "Failed to change user role.");
                    return BadRequest();
                }

            }
            catch
            {
                _logger.LogError("Failed to change user role.");
                ModelState.AddModelError(string.Empty, "Failed to change user role.");
                return BadRequest("Failed to change user role.");
            }
        }

        private async Task NotifyAccountChangedAsync(string action, Guid userId)
        {
            await realtimeNotifier.EntityChangedAsync(new RealtimeEntityChanged
            {
                Entity = "Account",
                Action = action,
                Id = userId,
                UserIds = new[] { userId },
                Roles = new[] { UserRoles.Admin.ToString() }
            });
        }
    }
}
