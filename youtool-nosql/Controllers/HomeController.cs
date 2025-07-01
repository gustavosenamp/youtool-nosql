using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using youtool_nosql.Models;

namespace youtool_nosql.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Youtube");
        }

    }
}
