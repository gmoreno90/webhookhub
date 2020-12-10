using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebHookHub.Models.DB;

namespace WebHookHub.Controllers
{
    /// <summary>
    /// Logs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLogItems : ControllerBase
    {
        private readonly WebHookHubContext _context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public ApiLogItems(WebHookHubContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiLogItem>>> GetClients(DateTime fromDate, DateTime toData)
        {
            return await _context.ApiLogItems.Where(x => x.RequestTime >= fromDate && x.RequestTime <= toData).OrderBy(x => x.RequestTime).ToListAsync();
        }

    }
}
