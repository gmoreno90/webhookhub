using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebHookHub.Services
{
    public interface INotificationService
    {
        Task<bool> PostData(Models.PostDataContent postDataContent);
    }
    public class NotificationService : INotificationService
    {
        private readonly ILogger<NotificationService> _logger;
        private readonly Models.DB.WebHookHubContext _context;

        public NotificationService(ILogger<NotificationService> logger, Models.DB.WebHookHubContext context)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>
        /// Post Data
        /// </summary>
        /// <param name="postDataContent"></param>
        /// <returns></returns>
        public async Task<bool> PostData(Models.PostDataContent postDataContent)
        {
            try
            {
                List<string> listJobIds = new List<string>();
                var webhooks = _context.ClientEvents.Where(x => x.Event.Code == postDataContent.EventCode && x.Client.Code == postDataContent.ClientCode && x.Enable).ToList();
                if (webhooks != null && webhooks.Count > 0)
                {
                    foreach (var item in webhooks)
                    {
                        var jobId = BackgroundJob.Enqueue(() => SendData(item.PostUrl, item.UserName, item.PostUrl, postDataContent.PostData, postDataContent.ContentType));
                        listJobIds.Add(jobId);
                        _logger.LogInformation(postDataContent.ToString());
                    }
                }
                else
                {
                    _logger.LogInformation("Not Webhooks Configured");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " : " + ex.StackTrace);
                return false;
            }
            
        }
        /// <summary>
        /// Send Data to WebHooks
        /// </summary>
        /// <param name="urlToPost"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="dataToPost"></param>
        /// <returns></returns>
        private void SendData(string urlToPost, string userName, string passWord, string dataToPost, string contentType)
        {
            using (WebClient wc = new WebClient())
            {
                if(string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(passWord))
                {
                    NetworkCredential myCreds = new NetworkCredential(userName, passWord);
                    wc.Credentials = myCreds;
                }
                wc.Headers[HttpRequestHeader.ContentType] = contentType;
                string HtmlResult = wc.UploadString(new Uri(urlToPost), dataToPost);
            }
        }
    }
}
