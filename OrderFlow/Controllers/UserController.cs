using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(
            ILogger<UserController> logger,
            IAccountService accountService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _accountService = accountService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "User");
            }


            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Speditor"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Driver"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Driver" });
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _accountService.GetCurrentUserWithProfilesAsync(this.GetUserId());

            if (user == null)
            {
                return NotFound();
            }

            return View(user.MapProfileViewModel());
        }

        [HttpPatch]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([FromBody] EditProfileViewModel model)
        {
            if (!IsUserAuthenticated())
            {
                return Unauthorized();
            }
            var userId = this.GetUserId();
            var user = await _accountService.GetCurrentUserWithProfilesAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            model.AccountType = user.AccountType;

            if (!TryValidateModel(model))
            {
                var errors = ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

                return BadRequest(new { errors });
            }

            var result = await _accountService.UpdateUserAndProfileAsync(userId, model);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    errors = new
                    {
                        Identity = result.Errors.Select(error => error.Description).ToArray()
                    }
                });
            }

            return Ok(user.MapProfileViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }

            var user = await _accountService.GetCurrentUserWithProfilesAsync(this.GetUserId());

            if (user == null)
            {
                return NotFound();
            }

            return View(user.MapEditProfileViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "User");
            }

            var userId = this.GetUserId();

            var user = await _accountService.GetCurrentUserWithProfilesAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            model.AccountType = user.AccountType;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountService.UpdateUserAndProfileAsync(userId, model);

            if (result.Succeeded)
            {
                return RedirectToAction("Profile", "User");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                _logger.LogWarning($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return NotFound();
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(changePasswordViewModel);
            }

            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction(nameof(Profile), "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = this.GetUserId();
                var result = await _accountService.DeleteAsync(userId);

                if (!result.Succeeded)
                {
                    _logger.LogWarning($"Unexpected error occurred deleting user with ID '{userId}'.");
                    return BadRequest();
                }

                await _signInManager.SignOutAsync();

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                _logger.LogWarning($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
                return NotFound();
            }
        }
    }
}
