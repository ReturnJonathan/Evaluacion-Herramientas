using DesafioH.Modelos;

namespace DesafioH.App.Services
{
    public interface ITareasService
    {
        Task<IEnumerable<Tarea>> GetAllAsync();
        Task<IEnumerable<Tarea>> SearchAsync(string query);
        Task<Tarea?> GetByIdAsync(Guid id);
    }
}
