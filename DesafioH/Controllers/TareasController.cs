using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DesafioH.Modelos;

namespace DesafioH.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly string _connStr;

        public TareasController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        private IDbConnection Connection => new SqlConnection(_connStr);

        // GET api/tareas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Titulo, t.Descripcion, t.Estado, t.Prioridad, 
                    t.FechaVencimiento, t.FechaCreacion,
                    t.ProyectoId, t.UsuarioAsignadoId
                FROM Tarea t";
            using var db = Connection;
            var lista = await db.QueryAsync<Tarea>(sql);
            return Ok(lista);
        }

        // GET api/tareas/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            const string sql = @"
                SELECT 
                    t.Id, t.Titulo, t.Descripcion, t.Estado, t.Prioridad, 
                    t.FechaVencimiento, t.FechaCreacion,
                    t.ProyectoId, t.UsuarioAsignadoId
                  FROM Tarea t
                 WHERE t.Id = @Id";
            using var db = Connection;
            var tarea = await db.QuerySingleOrDefaultAsync<Tarea>(sql, new { Id = id });
            return tarea is null ? NotFound() : Ok(tarea);
        }

        // POST api/tareas
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tarea t)
        {
            t.Id = Guid.NewGuid();
            t.FechaCreacion = DateTime.UtcNow;

            const string sql = @"
                INSERT INTO Tarea 
                    (Id, Titulo, Descripcion, Estado, Prioridad, FechaVencimiento, FechaCreacion, ProyectoId, UsuarioAsignadoId)
                VALUES
                    (@Id, @Titulo, @Descripcion, @Estado, @Prioridad, @FechaVencimiento, @FechaCreacion, @ProyectoId, @UsuarioAsignadoId);";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, t);
            return afect == 1
                ? CreatedAtAction(nameof(Get), new { id = t.Id }, t)
                : StatusCode(500, "Error al crear la tarea.");
        }

        // PUT api/tareas/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Tarea t)
        {
            const string sql = @"
                UPDATE Tarea
                   SET Titulo             = @Titulo,
                       Descripcion        = @Descripcion,
                       Estado             = @Estado,
                       Prioridad          = @Prioridad,
                       FechaVencimiento   = @FechaVencimiento,
                       ProyectoId         = @ProyectoId,
                       UsuarioAsignadoId  = @UsuarioAsignadoId
                 WHERE Id = @Id;";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, new
            {
                t.Titulo,
                t.Descripcion,
                t.Estado,
                t.Prioridad,
                t.FechaVencimiento,
                t.ProyectoId,
                t.UsuarioAsignadoId,
                Id = id
            });
            return afect == 1 ? NoContent() : NotFound();
        }

        // DELETE api/tareas/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            const string sql = "DELETE FROM Tarea WHERE Id = @Id";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, new { Id = id });
            return afect == 1 ? NoContent() : NotFound();
        }
    }
}
