using DesafioH.Modelos;

namespace DesafioH.App.Services
{
    public class TareasService : ITareasService
    {
        private readonly HttpClient _http;

        public TareasService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<Tarea>> GetAllAsync()
        {
            // Llama a GET /api/tareas
            var tareas = await _http.GetFromJsonAsync<IEnumerable<Tarea>>("api/tareas");
            return Map(tareas);
        }

        public async Task<IEnumerable<Tarea>> SearchAsync(string query)
        {
            // Si tu API tiene endpoint de búsqueda, úsalo; 
            // Si no, filtramos en cliente:
            var tareas = await _http.GetFromJsonAsync<IEnumerable<Tarea>>("api/tareas");
            var q = query.Trim().ToLower();
            var filtradas = tareas.Where(c =>
                c.Titulo.ToLower().Contains(q) ||
                (c.Proyecto != null && c.Proyecto.Nombre.ToLower().Contains(q)) ||
                (c.UsuarioAsignado != null && c.UsuarioAsignado.Nombre.ToLower().Contains(q))
            );
            return Map(filtradas);
        }


        private IEnumerable<Tarea> Map(IEnumerable<Tarea>? lista)
        {
            if (lista == null) return Array.Empty<Tarea>();

            return lista.Select(c => new Tarea
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Estado = c.Estado,
                FechaCreacion = c.FechaCreacion
            });
        }

        public async Task<Tarea?> GetByIdAsync(Guid id)
        {
            // GET /api/canciones/{id}
            var c = await _http.GetFromJsonAsync<Tarea>($"api/canciones/{id}");
            if (c == null) return null;
            return new Tarea
            {
                Id = c.Id,
                Titulo = c.Titulo,
                Descripcion = c.Descripcion,
                Estado = c.Estado,
                FechaCreacion = c.FechaCreacion
            };
        }

    }
}
