using DesafioH.Modelos;

namespace DesafioH.App.Models.Reports
{
    public class ReportsIndexVM
    {
        public string? Query { get; set; }
        public EstadoTarea? Estado { get; set; }
        public int? Prioridad { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public IEnumerable<Tarea> Tareas { get; set; } = Array.Empty<Tarea>();

    }
}
