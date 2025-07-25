using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace DesafioH.App.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _api;

        public UsuariosController(IHttpClientFactory http)
        {
            _api = http.CreateClient("Api");
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _api.GetFromJsonAsync<List<Usuario>>("/api/usuarios");
            return View(lista);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var usuario = await _api.GetFromJsonAsync<Usuario>($"/api/usuarios/{id}");
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.PostAsJsonAsync("/api/usuarios", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var usuario = await _api.GetFromJsonAsync<Usuario>($"/api/usuarios/{id}");
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Usuario model)
        {
            if (!ModelState.IsValid) return View(model);
            await _api.PutAsJsonAsync($"/api/usuarios/{id}", model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var usuario = await _api.GetFromJsonAsync<Usuario>($"/api/usuarios/{id}");
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _api.DeleteAsync($"/api/usuarios/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
