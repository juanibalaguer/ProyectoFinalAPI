using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class Productos_ServiciosController : ControllerBase
    {
        private readonly DataContext _context;

        public Productos_ServiciosController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Productos_Servicios/Productos
        [HttpGet("Productos")]
        public async Task<ActionResult<IEnumerable<Producto_Servicio>>> GetProductos()
        {
            try
            {
                var productos = await _context.Productos_Servicios
                    .Where(producto_servicio => producto_servicio.Consumible == 1)
                    .ToListAsync();
                if (productos.Count > 0)
                {
                    return Ok(productos);
                } else
                {
                    return NotFound("No se encontraron productos");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
        // GET: api/Productos_Servicios/Servicios
        [HttpGet("Servicios")]
        public async Task<ActionResult<IEnumerable<Producto_Servicio>>> GetServicios()
        {
            try
            {
                var servicios = await _context.Productos_Servicios
                    .Where(producto_servicio => producto_servicio.Consumible == 0)
                    .ToListAsync();
                if (servicios.Count > 0)
                {
                    return Ok(servicios);
                }
                else
                {
                    return NotFound("No se encontraron servicios");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // GET: api/Productos_Servicios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto_Servicio>> GetProducto_Servicio(int id)
        {
            try
            {
                var producto_Servicio = await _context.Productos_Servicios.FindAsync(id);
                if (producto_Servicio == null)
                {
                    return NotFound("El producto/servicio especificado no existe");
                }
                return Ok(producto_Servicio);
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        // PUT: api/Productos_Servicios/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "Empleado")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto_Servicio(int id, Producto_Servicio producto_Servicio)
        {
            try
            {
                if (Producto_ServicioExists(id))
                {
                    _context.Entry(producto_Servicio).State = EntityState.Modified;
                    _context.Entry(producto_Servicio)
                        .Property(producto_Servicio => producto_Servicio.Consumible)
                        .IsModified = false;
                    _context.Entry(producto_Servicio)
                        .Property(producto_Servicio => producto_Servicio.Nombre)
                        .IsModified = false;
                    _context.Entry(producto_Servicio)
                        .Property(producto_Servicio => producto_Servicio.Consumible)
                        .IsModified = false;
                    _context.Entry(producto_Servicio)
                        .Property(producto_Servicio => producto_Servicio.Descripcion)
                        .IsModified = false;
                    await _context.SaveChangesAsync();
                    return Ok(producto_Servicio);
                } else
                {
                    return NotFound("El producto/servicio especificado no existe");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        private bool Producto_ServicioExists(int id)
        {
            return _context.Productos_Servicios.Any(e => e.Id == id);
        }
    }
}
