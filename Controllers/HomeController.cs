using EMPLOYEE_MANAGER.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using EMPLOYEE_MANAGER.Services;


namespace EMPLOYEE_MANAGER.Controllers
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
            return View();
        }

        public IActionResult Dashboard()
        {
            var registros = AuditoriaStore.Registros
                .OrderByDescending(r => r.FechaHora)
                .ToList();

            return View(registros);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
