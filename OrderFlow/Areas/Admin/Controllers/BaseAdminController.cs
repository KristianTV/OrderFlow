using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Controllers;

namespace OrderFlow.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, Speditor")]
    [Area("Admin")]
    public class BaseAdminController : BaseController
    {
    }
}
