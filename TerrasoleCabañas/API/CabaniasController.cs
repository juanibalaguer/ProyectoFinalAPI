using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerrasoleCabañas.Model;

namespace TerrasoleCabañas.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabaniasController : ControllerBase
    {
        private readonly DataContext _context;

        public CabaniasController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Cabanias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cabania>>> GetCabanias()
        {
            return await _context.Cabanias.ToListAsync();
        }

        // GET: api/Cabanias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cabania>> GetCabania(int id)
        {
            var cabania = await _context.Cabanias.FindAsync(id);

            if (cabania == null)
            {
                return NotFound();
            }

            return cabania;
        }

        // PUT: api/Cabanias/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCabania(int id, Cabania cabania)
        {
            if (id != cabania.Id)
            {
                return BadRequest();
            }

            _context.Entry(cabania).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabaniaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cabanias
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Cabania>> PostCabania(Cabania cabania)
        {
            _context.Cabanias.Add(cabania);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCabania", new { id = cabania.Id }, cabania);
        }

        // DELETE: api/Cabanias/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Cabania>> DeleteCabania(int id)
        {
            var cabania = await _context.Cabanias.FindAsync(id);
            if (cabania == null)
            {
                return NotFound();
            }

            _context.Cabanias.Remove(cabania);
            await _context.SaveChangesAsync();

            return cabania;
        }

        private bool CabaniaExists(int id)
        {
            return _context.Cabanias.Any(e => e.Id == id);
        }
    }
}
