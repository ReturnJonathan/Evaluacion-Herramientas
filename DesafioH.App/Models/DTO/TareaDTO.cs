namespace DesafioH.App.Models.DTO
{
    public class TareaDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }

        public int Estado { get; set; }
        public int Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }

        public DateTime FechaVencimiento { get; set; }
    }
}
