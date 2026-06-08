using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OrderFlow.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected bool IsUserAuthenticated()
        {
            return User.Identity?.IsAuthenticated ?? false;
        }

        protected string? GetUserId()
        {
            bool isAuthenticated = this.IsUserAuthenticated();
            if (!isAuthenticated)
            {
                return null;
            }

            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected string? GetUserEmail()
        {
            bool isAuthenticated = this.IsUserAuthenticated();
            if (!isAuthenticated)
            {
                return null;
            }
            return User.FindFirstValue(ClaimTypes.Email);
        }

        protected string? GetUserName()
        {
            bool isAuthenticated = this.IsUserAuthenticated();
            if (!isAuthenticated)
            {
                return null;
            }
            return User.FindFirstValue(ClaimTypes.Name);
        }

        protected string? GetUserRole()
        {
            bool isAuthenticated = this.IsUserAuthenticated();
            if (!isAuthenticated)
            {
                return null;
            }
            return User.FindFirstValue(ClaimTypes.Role);
        }

        protected string? GetIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
