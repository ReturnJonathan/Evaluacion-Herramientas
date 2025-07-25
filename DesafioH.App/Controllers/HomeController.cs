using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioH.App.Models;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DesafioH.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _api;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory http)
        {
            _logger = logger;
            _api = http.CreateClient("Api");
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _api.GetFromJsonAsync<List<Usuario>>("api/usuarios")
                              ?? new List<Usuario>();
            var proyectos = await _api.GetFromJsonAsync<List<Proyecto>>("api/proyectos")
                              ?? new List<Proyecto>();
            var tareas = await _api.GetFromJsonAsync<List<Tarea>>("api/tareas")
                              ?? new List<Tarea>();

            ViewBag.ProyectosCount = proyectos.Count;
            ViewBag.TareasCount = tareas.Count;

            return View(usuarios);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
