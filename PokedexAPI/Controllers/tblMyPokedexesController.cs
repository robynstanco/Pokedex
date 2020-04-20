using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pokedex.Data;
using Pokedex.Data.Models;

//AUTO GENERATED! todo use as reference, but probably want to DI the repository layer for some separation here. 
//dont think it needs a logic layer. maybe for validation...
namespace PokedexAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class tblMyPokedexesController : ControllerBase
    {
        private readonly POKEDEXDBContext _context;

        public tblMyPokedexesController(POKEDEXDBContext context)
        {
            _context = context;
        }

        // GET: api/tblMyPokedexes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<tblMyPokedex>>> GettblMyPokedex()
        {
            return await _context.tblMyPokedex.ToListAsync();
        }

        // GET: api/tblMyPokedexes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<tblMyPokedex>> GettblMyPokedex(Guid id)
        {
            var tblMyPokedex = await _context.tblMyPokedex.FindAsync(id);

            if (tblMyPokedex == null)
            {
                return NotFound();
            }

            return tblMyPokedex;
        }

        // PUT: api/tblMyPokedexes/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PuttblMyPokedex(Guid id, tblMyPokedex tblMyPokedex)
        {
            if (id != tblMyPokedex.Id)
            {
                return BadRequest();
            }

            _context.Entry(tblMyPokedex).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tblMyPokedexExists(id))
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

        // POST: api/tblMyPokedexes
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<tblMyPokedex>> PosttblMyPokedex(tblMyPokedex tblMyPokedex)
        {
            _context.tblMyPokedex.Add(tblMyPokedex);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (tblMyPokedexExists(tblMyPokedex.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GettblMyPokedex", new { id = tblMyPokedex.Id }, tblMyPokedex);
        }

        // DELETE: api/tblMyPokedexes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<tblMyPokedex>> DeletetblMyPokedex(Guid id)
        {
            var tblMyPokedex = await _context.tblMyPokedex.FindAsync(id);
            if (tblMyPokedex == null)
            {
                return NotFound();
            }

            _context.tblMyPokedex.Remove(tblMyPokedex);
            await _context.SaveChangesAsync();

            return tblMyPokedex;
        }

        private bool tblMyPokedexExists(Guid id)
        {
            return _context.tblMyPokedex.Any(e => e.Id == id);
        }
    }
}
