using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DesafioH.Api.Consumer;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid id)
        {
            var tarea = await _api.GetFromJsonAsync<Tarea>($"/api/tareas/{id}");
            if (tarea == null) return NotFound();
            return View(tarea);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Proyectos = await GetProyectosAsync();
            ViewBag.Usuarios = await GetUsuariosAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tarea model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Proyectos = await GetProyectosAsync();
                ViewBag.Usuarios = await GetUsuariosAsync();
                return View(model);
            }

            model.Id = Guid.NewGuid();
            model.FechaCreacion = DateTime.UtcNow;

            var response = await _api.PostAsJsonAsync("/api/tareas", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "No se pudo crear la tarea.");
                ViewBag.Proyectos = await GetProyectosAsync();
                ViewBag.Usuarios = await GetUsuariosAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetProyectosAsync()
        {
            var proyectos = await _api.GetFromJsonAsync<List<Proyecto>>("/api/proyectos");
            return proyectos!
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Nombre })
                .ToList();
        }

        private async Task<List<SelectListItem>> GetUsuariosAsync()
        {
            var usuarios = await _api.GetFromJsonAsync<List<Usuario>>("/api/usuarios");
            return usuarios!
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Nombre })
                .ToList();
        }
        //public IActionResult Create() => View();



        //[HttpPost]
        //public async Task<IActionResult> Create(Tarea model)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    await _api.PostAsJsonAsync("/api/tareas", model);
        //    return RedirectToAction(nameof(Index));
        //}


        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tarea = await _api.GetFromJsonAsync<Tarea>($"/api/tareas/{id}");
            if (tarea == null) return NotFound();
            ViewBag.Proyectos = await GetProyectosAsync();
            ViewBag.Usuarios = await GetUsuariosAsync();
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
