using Microsoft.AspNetCore.Mvc;

namespace OrderFlow.Areas.Driver.Controllers
{
    public class DashboardController : BaseDriverController
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
