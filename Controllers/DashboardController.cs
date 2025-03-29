using Microsoft.AspNetCore.Mvc;
using EMPLOYEE_MANAGER.Services;

namespace EMPLOYEE_MANAGER.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var registros = AuditoriaStore.Registros
                .OrderByDescending(r => r.FechaHora)
                .ToList();

            return View(registros); // Pasamos la lista como modelo
        }
    }
}
