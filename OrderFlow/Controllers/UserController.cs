using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Data.Models;
using OrderFlow.Helpers;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.User;

namespace OrderFlow.Controllers
{
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IAccountService _accountService;
        private readonly IMailService _mailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(
            ILogger<UserController> logger,
            IAccountService accountService,
            IMailService mailService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _accountService = accountService;
            _mailService = mailService;
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
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    _logger.LogWarning($"Unable to load user with email '{model.Email}' after registration.");
                    return NotFound();
                }

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, token = emailConfirmationToken }, protocol: HttpContext.Request.Scheme) ?? "#";

                var privacyPolicyUrl = Url.Action("Privacy", "", new { }, protocol: HttpContext.Request.Scheme) ?? "#";

                await _mailService.SendMailAsync(model.Email, "Confirm your email", EmailTemplates.ConfirmEmail(callbackUrl, privacyPolicyUrl));

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
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToPage("/Index");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound($"Unable to load user with ID '{userId}'.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return View("ConfirmEmailError");

            return View("ConfirmEmailSuccess");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "You must confirm your email before logging in.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        if (CanUserAccessUrl(returnUrl, await _userManager.GetRolesAsync(user)))
                        {
                            return Redirect(returnUrl);
                        }
                    }

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

        private bool CanUserAccessUrl(string url, IList<string> roles)
        {
            var path = url.ToLower();

            if (path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
            {
                return roles.Contains("Admin") || roles.Contains("Speditor");
            }

            if (path.StartsWith("/Driver", StringComparison.OrdinalIgnoreCase))
            {
                return roles.Contains("Driver");
            }

            return true;
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

            var updatedUser = await _accountService.GetCurrentUserWithProfilesAsync(userId);

            if (updatedUser == null)
            {
                return NotFound();
            }

            await _signInManager.RefreshSignInAsync(updatedUser);

            return Ok(updatedUser.MapProfileViewModel());
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
        public async Task<IActionResult> ChangePassword()
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);

            if (!isTokenValid)
            {
                return BadRequest();
            }

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token))
            {
                return BadRequest();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return NotFound();
            }

            var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.Token);

            if (!isTokenValid)
            {
                return BadRequest();
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Action("ResetPassword", "User", new { token = token, email = model.Email }, protocol: HttpContext.Request.Scheme) ?? "#";
            string privacyPolicyUrl = Url.Action("Privacy", "", new { }, protocol: HttpContext.Request.Scheme) ?? "#";

            await _mailService.SendMailAsync(user.Email!, "Password Reset Successful", EmailTemplates.PasswordChanged(this.GetUserName() ?? "User", DateTime.UtcNow.ToString("f"), this.GetIpAddress() ?? "", callbackUrl, privacyPolicyUrl));

            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                    return BadRequest();

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "User", new { token = token, email = model.Email }, protocol: HttpContext.Request.Scheme) ?? "#";
                var privacyPolicyUrl = Url.Action("Privacy", "", new { }, protocol: HttpContext.Request.Scheme) ?? "#";

                await _mailService.SendMailAsync(user.Email!, "Reset Password", EmailTemplates.ResetPassword(callbackUrl, privacyPolicyUrl));

                return RedirectToAction("Login", "User");
            }
            catch (Exception)
            {
                return BadRequest();
            }

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
