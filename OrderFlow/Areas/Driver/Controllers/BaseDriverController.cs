using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderFlow.Controllers;

namespace OrderFlow.Areas.Driver.Controllers
{
    [Authorize(Roles = "Driver")]
    [Area("Driver")]
    public class BaseDriverController : BaseController
    {
    }
}
