using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using WebHookHub.Services;

namespace WebHookHub.Controllers
{
    /// <summary>
    /// WebHook POST DATA
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {

        private readonly ILogger<WebHookController> _logger;
        private readonly INotificationService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public WebHookController(ILogger<WebHookController> logger, INotificationService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EventCode">Event Code String</param>
        /// <param name="ClientCode">Cliente Code String</param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostData/{EventCode}/{ClientCode}")]
        public async Task<string> PostData(string EventCode, string ClientCode)
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
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return ex.Message;
            }

        }
        /// <summary>
        /// Helper to readRequest Body
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
