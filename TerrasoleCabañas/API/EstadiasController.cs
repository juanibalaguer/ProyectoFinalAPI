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
    public class EstadiasController : ControllerBase
    {
        private readonly DataContext _context;

        public EstadiasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Estadias
        [Authorize(Policy = "Inquilino")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Estadia>>> GetEstadia()
        {
            try
            {
                var inquilino = await _context.Inquilinos.Where(inquilino => inquilino.Email == User.Identity.Name).FirstOrDefaultAsync();
                var estadia = await _context.Estadias
                    .Include(estadia => estadia.Inquilino)
                    .Where(estadia => estadia.InquilinoId == inquilino.Id && estadia.FechaDesde <= DateTime.Now && estadia.FechaHasta >= DateTime.Now)
                    .FirstOrDefaultAsync();
                if (estadia != null)
                {
                    return Ok(estadia);
                } else
                {
                    return NotFound();
                }

            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }


       
        private bool CabaniaExists(int id)
        {
            return _context.Cabanias.Any(e => e.Id == id);
        }
    }
}
