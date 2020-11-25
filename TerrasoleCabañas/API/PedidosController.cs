using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly DataContext _context;

        public PedidosController(DataContext context)
        {
            _context = context;
        }
        // GET: api/Pedidos/5
        [Authorize(Policy = "Inquilino")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            try
            {
                var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
                var estadia = await _context.Estadias
                    .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                    .FirstOrDefaultAsync();
                var pedido = await _context.Pedidos.FindAsync(id);
                if (pedido != null && pedido.EstadiaId == estadia.Id)
                {
                    return Ok(pedido);
                }
                else return NotFound("No se encontró el pedido");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/PedidosPendientes
        [Authorize(Policy = "Empleado")]
        [HttpGet("PedidosPendientes")]
        public async Task<ActionResult<Pedido>> GetPedidosPendientes()
        {
            try
            {
                var pedidos = await _context.Pedidos
                    .Include(pedido => pedido.Estadia)
                    .Include(pedido => pedido.PedidoLineas)
                    .ThenInclude(pedidoLinea => pedidoLinea.Producto_Servicio)
                    .Where(pedido => pedido.Estado != 0 && pedido.Estado != 3).ToListAsync();
                if (pedidos.Count > 0)
                {
                    return Ok(pedidos);
                }
                else return NotFound("No hay pedidos pendientes");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET: api/Pedidos/Inquilino/
        [Authorize(Policy = "Inquilino")]
        [HttpGet("Inquilino/")]
        public async Task<ActionResult<Pedido>> GetPedidosInquilino()
        {
            try
            {
                var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
                var estadia = await _context.Estadias
                    .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                    .FirstOrDefaultAsync();
                var pedidos = await _context.Pedidos
                    .Include(pedido => pedido.PedidoLineas)
                    .ThenInclude(pedidoLinea => pedidoLinea.Producto_Servicio)
                    .Where(pedido => pedido.EstadiaId == estadia.Id).ToListAsync();
                if (pedidos.Count > 0)
                {
                    return Ok(pedidos);
                }
                else return NotFound("No hay pedidos pendientes");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT: api/Pedidos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut] // Empleado e inquilino
        public async Task<IActionResult> PutPedido(Pedido pedido)
        {
            try
            {
                if (User.IsInRole("Empleado"))
                {
                    if (PedidoExists(pedido.Id))
                    {
                        _context.Entry(pedido).State = EntityState.Modified;
                        _context.Entry(pedido).Property(pedido => pedido.MontoPedido).IsModified = false;
                        _context.Entry(pedido).Property(pedido => pedido.FechaPedido).IsModified = false;
                        _context.Entry(pedido).Property(pedido => pedido.EstadiaId).IsModified = false;
                        await _context.SaveChangesAsync();
                        return Ok(pedido);
                    }
                    else return NotFound("No existe el pedido");

                }
                else
                {
                    var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
                    var estadia = await _context.Estadias
                        .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                        .FirstOrDefaultAsync();
                    var pedidoACambiar = await _context.Pedidos
                        .Include(p => p.PedidoLineas)
                        .Where(p => p.Id == pedido.Id && p.EstadiaId == estadia.Id && p.Estado <= 2) // Se verifica que el pedido sea de esa estancia(de ese inquilino) y que no esté en preparación o ya entregado.
                        .FirstOrDefaultAsync();
                    if (pedido.FechaPedido < DateTime.Now || pedido.FechaPedido < estadia.FechaDesde || pedido.FechaPedido > estadia.FechaHasta)
                    {
                        return BadRequest("Seleccione una fecha válida");
                    }
                    if (pedidoACambiar != null)
                    {
                        _context.Entry(pedidoACambiar).CurrentValues.SetValues(pedido);
                        //Borrar lineasPedido que se hayan borrado
                        foreach (PedidoLinea pedidoLineaExistente in pedidoACambiar.PedidoLineas)
                        {
                            if (!pedido.PedidoLineas.Any(pedidoLinea => pedidoLinea.Id == pedidoLineaExistente.Id))
                                _context.PedidoLineas.Remove(pedidoLineaExistente);
                        }
                        List<PedidoLinea> pedidoLineasNuevas = new List<PedidoLinea>();
                        foreach (PedidoLinea pedidoLinea in pedido.PedidoLineas)
                        {
                            var pedidoLineaExistente = pedidoACambiar.PedidoLineas
                                .Where(p => p.Id == pedidoLinea.Id)
                                .SingleOrDefault();

                            if (pedidoLineaExistente != null)
                            {
                                // Actualizar línea
                                _context.Entry(pedidoLineaExistente).CurrentValues.SetValues(pedidoLinea);
                            }
                            else
                            {
                               
                                pedidoLineasNuevas.Add(pedidoLinea);
                            }
                        }
                        if(pedidoLineasNuevas.Count > 0) {
                            foreach (PedidoLinea pedidoLineaNueva in pedidoLineasNuevas)
                            {
                                pedidoACambiar.PedidoLineas.Add(pedidoLineaNueva);
                                _context.Entry(pedidoLineaNueva.Producto_Servicio).State = EntityState.Unchanged;
                            }
                        }
                        
                        await _context.SaveChangesAsync();
                        return Ok(pedido);
                    }
                    else
                    {
                        return BadRequest("No se puede modificar el pedido solicitado");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        // POST: api/Pedidos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Policy = "Inquilino")]
        [HttpPost]
        public async Task<ActionResult<Pedido>> PostPedido(Pedido pedido)
        {
            var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
            var estadia = await _context.Estadias
                .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                .FirstOrDefaultAsync();
            if (pedido.FechaPedido < DateTime.Now || pedido.FechaPedido < estadia.FechaDesde || pedido.FechaPedido > estadia.FechaHasta)
            {
                return BadRequest("Seleccione una fecha válida");
            }
            _context.Pedidos.Add(pedido);
            foreach (PedidoLinea pedidoLinea in pedido.PedidoLineas)
            {
                _context.Entry(pedidoLinea.Producto_Servicio).State = EntityState.Unchanged;
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetPedido", new { id = pedido.Id }, pedido);
        }

        // DELETE: api/Pedidos/5
        [Authorize(Policy = "Inquilino")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Pedido>> DeletePedido(int id)
        {
            var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
            var estadia = await _context.Estadias
                .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                .FirstOrDefaultAsync();
            var pedido = await _context.Pedidos.Include(pedido => pedido.PedidoLineas).Where(pedido => pedido.Id == id).FirstOrDefaultAsync();
            if (pedido == null || pedido.EstadiaId != estadia.Id)
            {
                return NotFound("No se encontró o no se puede borrar el pedido solicitado");
            }
            if(pedido.Estado == 2 || pedido.Estado == 3)
            {
                return BadRequest("No se puede cancelar un pedido en prepración");
            }
            foreach(PedidoLinea pedidoLinea in pedido.PedidoLineas)
            {
                _context.PedidoLineas.Remove(pedidoLinea);
            }
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return Ok("Pedido eliminado");
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }

     
    }
}
