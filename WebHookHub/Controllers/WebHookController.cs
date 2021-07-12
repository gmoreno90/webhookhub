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
        /// Post Data
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
                string strRQ = "";
                using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    strRQ = await reader.ReadToEndAsync();
                }
                
                return await _service.PostData(new Models.PostDataContent()
                {
                    EventCode = EventCode,
                    ClientCode = ClientCode,
                    PostData = strRQ,
                    ContentType = Request.ContentType,
                    DelayMode = Models.DelayMode.Instant,
                    DelayValue = 0D
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return ex.Message;
            }

        }
        /// <summary>
        /// Post Data Delayed
        /// </summary>
        /// <param name="EventCode"></param>
        /// <param name="ClientCode"></param>
        /// <param name="DelayMode"></param>
        /// <param name="DelayValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PostDataDelayed/{EventCode}/{ClientCode}/{DelayMode}/{DelayValue}")]
        public async Task<string> PostDataDelayed(string EventCode, string ClientCode, Models.DelayMode DelayMode, decimal DelayValue)
        {
            try
            {
                string strRQ = "";
                using (StreamReader reader = new StreamReader(Request.Body, System.Text.Encoding.UTF8))
                {
                    strRQ = await reader.ReadToEndAsync();
                }

                return await _service.PostData(new Models.PostDataContent()
                {
                    EventCode = EventCode,
                    ClientCode = ClientCode,
                    PostData = strRQ,
                    ContentType = Request.ContentType,
                    DelayMode = DelayMode,
                    DelayValue = (double?)DelayValue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return ex.Message;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="JobID">JobID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DeleteJob/{JobID}")]
        public async Task<bool> DeleteJob(string JobID)
        {
            try
            {
                return await _service.DeleteJob(JobID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return false;
            }

        }

    }
}
