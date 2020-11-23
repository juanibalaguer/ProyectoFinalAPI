using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PedidoLineasController : ControllerBase
    {
        private readonly DataContext _context;

        public PedidoLineasController(DataContext context)
        {
            _context = context;
        }

        // PUT: api/PedidoLineas/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Policy = "Inquilino")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedidoLinea(int id, PedidoLinea pedidoLinea)
        {
            try
            {

                var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
                var estadia = await _context.Estadias
                    .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                    .FirstOrDefaultAsync();
                var producto_servicio = await _context.Productos_Servicios.FindAsync(pedidoLinea.Producto_ServicioId);
                var pedido = await _context.Pedidos.FindAsync(pedidoLinea.PedidoId);
                if (pedido.EstadiaId == estadia.Id && pedido.Estado < 2)
                {
                    _context.Entry(pedidoLinea).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Ok(pedidoLinea);
                }
                else
                {
                    return BadRequest("No puede modificar ese ítem");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // POST: api/PedidoLineas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "Inquilino")]
        [HttpPost]
        public async Task<ActionResult<PedidoLinea>> PostPedidoLinea(PedidoLinea pedidoLinea)
        {
            _context.Attach(pedidoLinea);
            _context.Entry(pedidoLinea).State = EntityState.Added;
            _context.Entry(pedidoLinea.Producto_Servicio).State = EntityState.Unchanged;
          await _context.SaveChangesAsync();
            return CreatedAtAction("GetPedidoLinea", new { id = pedidoLinea.Id }, pedidoLinea);
        }

        // DELETE: api/PedidoLineas/5
        [Authorize(Policy = "Inquilino")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<PedidoLinea>> DeletePedidoLinea(int id)
        {
            var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
            var estadia = await _context.Estadias
                .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                .FirstOrDefaultAsync();
            var pedidoLinea = await _context.PedidoLineas.FindAsync(id);
            if (pedidoLinea == null)
            {
                return NotFound();
            }
            var pedido = await _context.Pedidos.FindAsync(pedidoLinea.PedidoId);
            if (pedido.EstadiaId == estadia.Id && pedido.Estado >= 2)
            {
                _context.PedidoLineas.Remove(pedidoLinea);
                await _context.SaveChangesAsync();
                return Ok(pedidoLinea);
            }
            else
            {
                return BadRequest("No puede eliminar ese ítem");
            }



        }

        private bool PedidoLineaExists(int id)
        {
            return _context.PedidoLineas.Any(e => e.Id == id);
        }
    }
}
