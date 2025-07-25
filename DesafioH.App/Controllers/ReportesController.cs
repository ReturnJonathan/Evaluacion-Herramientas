using System;
using System.Linq;
using System.Threading.Tasks;
using DesafioH.App.Models;
using DesafioH.App.Models.Reports;
using DesafioH.App.Services;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace DesafioH.App.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ITareasService _tareasService;

        public ReportesController(ITareasService tareasService)
        {
            _tareasService = tareasService;
        }

        public async Task<IActionResult> Index(string? q, EstadoTarea? estado, int? prioridad, DateTime? desde, DateTime? hasta)
        {
            var tareas = (await _tareasService.GetAllAsync()).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                tareas = tareas.Where(t => t.Titulo.Contains(q, StringComparison.OrdinalIgnoreCase));

            if (estado.HasValue)
                tareas = tareas.Where(t => t.Estado == estado.Value);

            if (prioridad.HasValue)
                tareas = tareas.Where(t => t.Prioridad == prioridad.Value);

            if (desde.HasValue)
                tareas = tareas.Where(t => t.FechaCreacion >= desde.Value);

            if (hasta.HasValue)
                tareas = tareas.Where(t => t.FechaCreacion <= hasta.Value);

            var vm = new ReportsIndexVM
            {
                Query = q,
                Estado = estado,
                Prioridad = prioridad,
                Desde = desde,
                Hasta = hasta,
                Tareas = tareas.ToList()
            };

            return View(vm);
        }
    }
}
