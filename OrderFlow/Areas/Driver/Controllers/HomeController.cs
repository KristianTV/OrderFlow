using Microsoft.AspNetCore.Mvc;
using OrderFlow.ViewModels.System;
using System.Diagnostics;


namespace OrderFlow.Areas.Driver.Controllers
{
    public class HomeController : BaseDriverController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                switch (statusCode.Value)
                {
                    case 404:
                        return View("NotFound");
                    case 500:
                        return View("BadRequest");
                }

            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
