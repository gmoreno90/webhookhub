using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<bool> PostData([FromBody]object Data, string EventCode, string ClientCode)
        {
            try
            {

                string strRQ = await ReadRequestBody(Request);

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
        private async Task<string> ReadRequestBody(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            //request.EnableRewind();
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = System.Text.Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }
}
