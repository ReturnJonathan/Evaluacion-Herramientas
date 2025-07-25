using System;
using System.ComponentModel.DataAnnotations;

namespace DesafioH.Modelos
{
    public class Tarea
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required]
        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;

        public int Prioridad { get; set; } = 0;

        public DateTime? FechaVencimiento { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        public Guid? UsuarioAsignadoId { get; set; }
        public Usuario? UsuarioAsignado { get; set; }
    }
}
