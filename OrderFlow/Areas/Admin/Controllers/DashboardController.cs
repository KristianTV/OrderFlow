using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class DashboardController : BaseAdminController
    {

        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ILogger<DashboardController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
