using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHookHub.Models.DB;

namespace WebHookHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientEventWebhooksController : ControllerBase
    {
        private readonly WebHookHubContext _context;

        public ClientEventWebhooksController(WebHookHubContext context)
        {
            _context = context;
        }

        // GET: api/ClientEventWebhooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientEventWebhooks>>> GetClientEventWebhooks()
        {
            return await _context.ClientEventWebhooks.ToListAsync();
        }

        // GET: api/ClientEventWebhooks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientEventWebhooks>> GetClientEventWebhooks(int id)
        {
            var clientEventWebhooks = await _context.ClientEventWebhooks.FindAsync(id);

            if (clientEventWebhooks == null)
            {
                return NotFound();
            }

            return clientEventWebhooks;
        }

        // PUT: api/ClientEventWebhooks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClientEventWebhooks(int id, ClientEventWebhooks clientEventWebhooks)
        {
            if (id != clientEventWebhooks.ID)
            {
                return BadRequest();
            }

            _context.Entry(clientEventWebhooks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientEventWebhooksExists(id))
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

        // POST: api/ClientEventWebhooks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ClientEventWebhooks>> PostClientEventWebhooks(ClientEventWebhooks clientEventWebhooks)
        {
            _context.ClientEventWebhooks.Add(clientEventWebhooks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClientEventWebhooks", new { id = clientEventWebhooks.ID }, clientEventWebhooks);
        }

        // DELETE: api/ClientEventWebhooks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientEventWebhooks(int id)
        {
            var clientEventWebhooks = await _context.ClientEventWebhooks.FindAsync(id);
            if (clientEventWebhooks == null)
            {
                return NotFound();
            }

            _context.ClientEventWebhooks.Remove(clientEventWebhooks);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientEventWebhooksExists(int id)
        {
            return _context.ClientEventWebhooks.Any(e => e.ID == id);
        }
    }
}
