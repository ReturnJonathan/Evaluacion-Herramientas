using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace DesafioH.App.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly HttpClient _api;

        public ProyectosController(IHttpClientFactory http)
        {
            _api = http.CreateClient("Api");
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetFromJsonAsync<List<Proyecto>>("/api/proyectos");
            return View(lista);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var proyecto = await _api.GetFromJsonAsync<Proyecto>($"/api/proyectos/{id}");
            if (proyecto == null) return NotFound();
            return View(proyecto);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Proyecto model)
        {
            if (!ModelState.IsValid) return View(model);
            var resp = await _api.PostAsJsonAsync("/api/Conductores", model);
            if (resp.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            ModelState.AddModelError("", "No se pudo crear.");
            return View(model);
        }


        public async Task<IActionResult> Edit(Guid id)
        {
            var proyecto = await _api.GetFromJsonAsync<Proyecto>($"/api/proyectos/{id}");
            if (proyecto == null) return NotFound();
            return View(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Proyecto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.PutAsJsonAsync($"/api/proyectos/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var proyecto = await _api.GetFromJsonAsync<Proyecto>($"/api/proyectos/{id}");
            if (proyecto == null) return NotFound();
            return View(proyecto);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _api.DeleteAsync($"/api/proyectos/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
