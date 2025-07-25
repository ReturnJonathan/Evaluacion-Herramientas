using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace DesafioH.App.Controllers
{
    public class TareasController : Controller
    {
        private readonly HttpClient _api;

        public TareasController(IHttpClientFactory http)
        {
            _api = http.CreateClient("Api");
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetFromJsonAsync<List<Tarea>>("/api/tareas");
            return View(lista);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var tarea = await _api.GetFromJsonAsync<Tarea>($"/api/tareas/{id}");
            if (tarea == null) return NotFound();
            return View(tarea);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Tarea model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.PostAsJsonAsync("/api/tareas", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var tarea = await _api.GetFromJsonAsync<Tarea>($"/api/tareas/{id}");
            if (tarea == null) return NotFound();
            return View(tarea);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Tarea model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.PutAsJsonAsync($"/api/tareas/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var tarea = await _api.GetFromJsonAsync<Tarea>($"/api/tareas/{id}");
            if (tarea == null) return NotFound();
            return View(tarea);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _api.DeleteAsync($"/api/tareas/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
