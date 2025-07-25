using DesafioH.Api.Consumer;
using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc;

namespace DesafioH.App.Controllers
{
    public class ProyectosController : Controller
    {
        public IActionResult Index()
        {
            var data = Crud<Proyecto>.GetAll();
            ViewBag.TotalRegistros = data.Count;
            ViewBag.TotalTareas = Crud<Tarea>.GetAll().Count;
            return View(data);
        }
    }
}
