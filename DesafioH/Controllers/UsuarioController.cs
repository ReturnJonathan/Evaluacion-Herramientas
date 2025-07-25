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
    public class UsuariosController : ControllerBase
    {
        private readonly string _connStr;

        public UsuariosController(IConfiguration config)
        {
            _connStr = config.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Cadena no configurada o error.");
        }

        private IDbConnection Connection => new SqlConnection(_connStr);

        // GET api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string sql = "SELECT Id, Nombre, Email, PasswordHash FROM Usuario";
            using var db = Connection;
            var lista = await db.QueryAsync<Usuario>(sql);
            return Ok(lista);
        }

        // GET api/usuarios/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            const string sql = @"
                SELECT Id, Nombre, Email, PasswordHash
                  FROM Usuario
                 WHERE Id = @Id";
            using var db = Connection;
            var usuario = await db.QuerySingleOrDefaultAsync<Usuario>(sql, new { Id = id });
            return usuario is null ? NotFound() : Ok(usuario);
        }

        // POST api/usuarios
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Usuario u)
        {
            u.Id = Guid.NewGuid();
            // En un caso real deberías hashear u.PasswordHash aquí
            const string sql = @"
                INSERT INTO Usuario (Id, Nombre, Email, PasswordHash)
                VALUES (@Id, @Nombre, @Email, @PasswordHash);";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, u);
            return afect == 1
                ? CreatedAtAction(nameof(Get), new { id = u.Id }, u)
                : StatusCode(500, "Error al crear el usuario.");
        }

        // PUT api/usuarios/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Usuario u)
        {
            const string sql = @"
                UPDATE Usuario
                   SET Nombre       = @Nombre,
                       Email        = @Email,
                       PasswordHash = @PasswordHash
                 WHERE Id = @Id;";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, new
            {
                u.Nombre,
                u.Email,
                u.PasswordHash,
                Id = id
            });
            return afect == 1 ? NoContent() : NotFound();
        }

        // DELETE api/usuarios/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            const string sql = "DELETE FROM Usuario WHERE Id = @Id";
            using var db = Connection;
            var afect = await db.ExecuteAsync(sql, new { Id = id });
            return afect == 1 ? NoContent() : NotFound();
        }

    }
}
