using Microsoft.AspNet.Identity;
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
    }
}
