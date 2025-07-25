using DesafioH.Modelos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DesafioH.App.Models
{
    public class TareaCreateVM
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        public EstadoTarea Estado { get; set; }

        public int Prioridad { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaCreacion { get; set; }
        public Guid ProyectoId { get; set; }
        public SelectList? Proyectos { get; set; }
        public Guid? UsuarioAsignadoId { get; set; }
    }
}
