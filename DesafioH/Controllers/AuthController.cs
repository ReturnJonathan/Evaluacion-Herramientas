using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using DesafioH.Modelos;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace DesafioH.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string _connStr;
        private readonly IConfiguration _cfg;

        public AuthController(IConfiguration cfg)
        {
            _cfg = cfg;
            _connStr = cfg.GetConnectionString("DefaultConnection")!;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            using var db = new SqlConnection(_connStr);
            var user = db.QuerySingleOrDefault<Usuario>(
                "SELECT * FROM Usuario WHERE Email = @Email",
                new { req.Email }
            );

            if (user == null || user.PasswordHash != req.Password)
                return Unauthorized();

            var jwtSec = _cfg.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSec["Key"]!);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

            var token = new JwtSecurityToken(
                issuer: jwtSec["Issuer"],
                audience: jwtSec["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSec["DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

}
