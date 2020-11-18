using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Inquilino")]
    [ApiController]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext _context;

        public InquilinosController(DataContext context)
        {
            _context = context;
        }
        // GET: api/Inquilinos/
        [HttpGet]
        public async Task<ActionResult<Inquilino>> GetInquilino()
        {
            try
            {

                var inquilino = await _context.Inquilinos
                    .Where(inquilino => inquilino.Email == User.Identity.Name)
                    .FirstOrDefaultAsync();
                if (inquilino == null)
                {
                    return NotFound();
                }

                return Ok(inquilino);
            } catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        // PUT: api/Inquilinos/
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> PutInquilino(Inquilino inquilino)
        {
                try
                {
                    if (_context.Inquilinos.AsNoTracking().FirstOrDefault(i => i.Email == User.Identity.Name) != null)
                    {
                        _context.Entry(inquilino).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        return Ok(inquilino);
                    }

                    return BadRequest("No se encontró el inquilino");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
        }
        private bool InquilinoExists(int id)
        {
            return _context.Inquilinos.Any(e => e.Id == id);
        }
    }
}
