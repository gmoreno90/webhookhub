using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHookHub.Models.DB;

namespace WebHookHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHooksController : ControllerBase
    {
        private readonly WebHookHubContext _context;

        public WebHooksController(WebHookHubContext context)
        {
            _context = context;
        }

        // GET: api/ClientEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientEvent>>> GetClientEvents()
        {
            return await _context.ClientEvents.ToListAsync();
        }

        // GET: api/ClientEvents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientEvent>> GetClientEvent(int id)
        {
            var clientEvent = await _context.ClientEvents.FindAsync(id);

            if (clientEvent == null)
            {
                return NotFound();
            }

            return clientEvent;
        }

        // PUT: api/ClientEvents/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientEvent(int id, ClientEvent clientEvent)
        {
            if (id != clientEvent.ID)
            {
                return BadRequest();
            }

            _context.Entry(clientEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientEventExists(id))
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

        // POST: api/ClientEvents
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ClientEvent>> PostClientEvent(ClientEvent clientEvent)
        {
            _context.ClientEvents.Add(clientEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientEvent", new { id = clientEvent.ID }, clientEvent);
        }

        // DELETE: api/ClientEvents/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ClientEvent>> DeleteClientEvent(int id)
        {
            var clientEvent = await _context.ClientEvents.FindAsync(id);
            if (clientEvent == null)
            {
                return NotFound();
            }

            _context.ClientEvents.Remove(clientEvent);
            await _context.SaveChangesAsync();

            return clientEvent;
        }

        private bool ClientEventExists(int id)
        {
            return _context.ClientEvents.Any(e => e.ID == id);
        }
    }
}
