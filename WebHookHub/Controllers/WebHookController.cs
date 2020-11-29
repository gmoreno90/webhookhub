using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebHookHub.Services;

namespace WebHookHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebHookController : ControllerBase
    {
        
        private readonly ILogger<WebHookController> _logger;
        private readonly INotificationService _service;

        public WebHookController(ILogger<WebHookController> logger, INotificationService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClientCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostData/{EventCode}/{ClientCode}")]
        public async Task<bool> PostData(string EventCode, string ClientCode)
        {
            try
            {
                Stream req = Request.Body;
                req.Seek(0, System.IO.SeekOrigin.Begin);
                string strRQ = new StreamReader(req).ReadToEnd();

                return await _service.PostData(new Models.PostDataContent()
                {
                    EventCode = EventCode,
                    ClientCode = ClientCode,
                    PostData = strRQ,
                    ContentType = Request.ContentType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }
    }
}
