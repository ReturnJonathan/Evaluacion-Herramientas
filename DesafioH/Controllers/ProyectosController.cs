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
    public class ProyectosController : ControllerBase
    {
        private readonly string _connStr;

        public ProyectosController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        private IDbConnection Connection => new SqlConnection(_connStr);

        // GET api/proyectos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = @"
                SELECT Id, Nombre, Descripcion, FechaCreacion
                  FROM Proyecto";
            using var db = Connection;
            var list = await db.QueryAsync<Proyecto>(sql);
            return Ok(list);
        }

        // GET api/proyectos/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            const string sql = @"
                SELECT Id, Nombre, Descripcion, FechaCreacion
                  FROM Proyecto
                 WHERE Id = @Id";
            using var db = Connection;
            var proyecto = await db.QuerySingleOrDefaultAsync<Proyecto>(sql, new { Id = id });
            return proyecto is null ? NotFound() : Ok(proyecto);
        }

        // POST api/proyectos
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proyecto p)
        {
            p.Id = Guid.NewGuid();
            p.FechaCreacion = DateTime.UtcNow;

            const string sql = @"
                INSERT INTO Proyecto (Id, Nombre, Descripcion, FechaCreacion)
                VALUES (@Id, @Nombre, @Descripcion, @FechaCreacion);";
            using var db = Connection;
            var affected = await db.ExecuteAsync(sql, p);
            return affected == 1
                ? CreatedAtAction(nameof(Get), new { id = p.Id }, p)
                : StatusCode(500, "Error al crear el proyecto.");
        }

        // PUT api/proyectos/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Proyecto p)
        {
            const string sql = @"
                UPDATE Proyecto
                   SET Nombre       = @Nombre,
                       Descripcion  = @Descripcion
                 WHERE Id = @Id;";
            using var db = Connection;
            var affected = await db.ExecuteAsync(sql, new { p.Nombre, p.Descripcion, Id = id });
            return affected == 1 ? NoContent() : NotFound();
        }

        // DELETE api/proyectos/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            const string sql = "DELETE FROM Proyecto WHERE Id = @Id";
            using var db = Connection;
            var affected = await db.ExecuteAsync(sql, new { Id = id });
            return affected == 1 ? NoContent() : NotFound();
        }
    }
}
