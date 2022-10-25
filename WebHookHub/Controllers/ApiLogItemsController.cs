using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class ApiLogItemsController : ControllerBase
    {
        private readonly WebHookHubContext _context;
        private readonly ILogger<ApiLogItemsController> _logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public ApiLogItemsController(WebHookHubContext context, ILogger<ApiLogItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get Logs from Dates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<ActionResult<IEnumerable<ApiLogItem>>> GetLogs(DateTime fromDate, DateTime toData)
        {
            return await _context.ApiLogItems.Where(x => x.RequestTime >= fromDate && x.RequestTime <= toData).OrderBy(x => x.RequestTime).ToListAsync();
        }

        /// <summary>
        /// Get Logs Data To Posts
        /// </summary>
        /// <param name="rq"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDataToPost")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<List<DataToPost>> GetDataToPost(Models.DataToPostRQ rq)
        {
            try
            {
                var query = _context.DataToPosts.AsQueryable();
                if (!string.IsNullOrEmpty(rq.ID)) {
                    query = query.Where(x => x.ID == rq.ID);
                }
                if (!string.IsNullOrEmpty(rq.ContentValue))
                {
                    query = query.Where(x => x.Content.Contains(rq.ContentValue));
                }
                if (!string.IsNullOrEmpty(rq.ContentID))
                {
                    query = query.Where(x => x.ContentID == rq.ContentID);
                }
                if (!string.IsNullOrEmpty(rq.ContentExtraID))
                {
                    query = query.Where(x => x.ContentExtraID == rq.ContentExtraID);
                }
                if (!string.IsNullOrEmpty(rq.EventCode))
                {
                    query = query.Where(x => x.EventCode == rq.EventCode);
                }
                if (!string.IsNullOrEmpty(rq.ClientCode))
                {
                    query = query.Where(x => x.ClientCode == rq.ClientCode);
                }
                var res = await query.OrderByDescending(x=>x.RequestDate).ToListAsync();
                if(res  != null)
                {
                    foreach (var item in res)
                    {
                        item.Content = Models.Utils.GeneralUtils.FromByteArray<string>(item.ContentBinary);
                    }
                }    
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                throw;
            }

        }


    }
}
