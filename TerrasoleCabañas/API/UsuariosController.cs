using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration iconfiguration;


        public UsuariosController(DataContext context, IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
            _context = context;
        }
        // GET: api/Usuarios/

        [HttpGet]
        public async Task<IActionResult> GetUsuario()
        {
            try
            {
                var usuario = await _context.Usuarios.Where(usuario => usuario.Email == User.Identity.Name).FirstOrDefaultAsync();
                if (usuario != null)
                {
                    return Ok(usuario);
                }
                else
                {
                    return NotFound("No se encontró el usuario");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutUsuario(Usuario usuario)
        {
            try
            {
                if (usuario.Email == User.Identity.Name)
                {
                    _context.Entry(usuario).State = EntityState.Modified;
                    _context.Entry(usuario)
                        .Property(usuario => usuario.Email)
                        .IsModified = false;
                    _context.Entry(usuario)
                       .Property(usuario => usuario.Contraseña)
                       .IsModified = false;
                    _context.Entry(usuario)
                       .Property(usuario => usuario.Rol)
                       .IsModified = false;
                    await _context.SaveChangesAsync();
                    return Ok(usuario);
                }
                else
                {
                    return BadRequest("El usuario que desea modificar no coincide con su usuario");
                }
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // POST: api/Usuarios
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "Administrador")]
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                var emailNoDisponible = await _context.Usuarios.Where(u => u.Email == usuario.Email).AnyAsync();
                if (emailNoDisponible)
                {
                    return BadRequest("El email ingresado no está disponible");
                }
                string hashContraseña = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: usuario.Contraseña,
                    salt: System.Text.Encoding.ASCII.GetBytes(iconfiguration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                usuario.Contraseña = hashContraseña;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/Usuarios/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [AllowAnonymous]
        [HttpPost("Login/")]
        public async Task<ActionResult<Usuario>> Login([FromForm] string email, [FromForm] string contraseña)
        {
            try
            {
                var usuario = await _context.Usuarios.Where(usuario => usuario.Email == email).FirstOrDefaultAsync();
                string hashContraseña = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: contraseña,
                    salt: System.Text.Encoding.ASCII.GetBytes(iconfiguration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                if (usuario == null || hashContraseña != usuario.Contraseña)
                {
                    return BadRequest("Usuario y/o contraseña incorrecto/s");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                       System.Text.Encoding.ASCII.GetBytes(iconfiguration["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", usuario.Nombre + " " + usuario.Apellido),
                        new Claim(ClaimTypes.Role, usuario.NombreRol()),
                    };

                    var token = new JwtSecurityToken(
                        issuer: iconfiguration["TokenAuthentication:Issuer"],
                        audience: iconfiguration["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(60),
                        signingCredentials: credenciales
                    );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/Usuarios/5
        [Authorize(Policy = "Administrador")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
