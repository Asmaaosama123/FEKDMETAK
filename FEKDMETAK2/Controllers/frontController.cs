using FEKDMETAK2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApplication5.Controllers
{
    public class frontController : Controller
    {
        private readonly ILogger<frontController> _logger;

        public frontController(ILogger<frontController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult service()
        {
            return View();
        }
        public IActionResult about()
        {
            return View();
        }
        public IActionResult contact()
        {
            return View();
        }
        public IActionResult team()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
